using DualBuffer.Models.Enums;
using System;

namespace DualBuffer.Services
{
    public class NetworkService
    {

        /* Queue<Call> buffer1 = new Queue<Call>();
         Queue<Call> buffer2 = new Queue<Call>();*/
        List<Call> buffer1 = new List<Call>();
        List<Call> buffer2 = new List<Call>();
        int buffer1Size = 5; // set the size of buffer 1 to 5
        int buffer2Size = 10;

        // Algorithm 1: Accepting Requests into the Network
        double deltaXi;
        double Xc;
        public  bool AcceptRequest(double Di, double SNR, double WRB, int N, int Nco)
        {
            double BA = Di * Math.Log(1 + SNR);
            double S = 1;
            double Bi = 1;
            deltaXi = S * Bi * WRB / 2;
            double totalDeltaXi = deltaXi;
            Xc = 0;

            for (int i = 2; i <= Nco; i++)
            {
                double ki = 1;
                double delta = ki * deltaXi;
                totalDeltaXi += delta;
            }

            Xc = totalDeltaXi <= N - Nco ? totalDeltaXi : 0;

            if (Xc + deltaXi <= N - Nco)
            {
                return true; // Accept request
            }
            else
            {
                return false; // Reject request
            }
        }

        // Algorithm 2: Accepting Requests into the Buffer
        public  void AcceptRequestIntoBuffer( Call incomingCall) 
        {
               double RTBlockingProb;
              double NRTBlockingProb;
             RTBlockingProb = 0;
            NRTBlockingProb = 0;


            if (buffer1.Count < buffer1Size)
            {
                buffer1.Add(incomingCall);
            }
            else
            {
                bool isRT = incomingCall.Type == CallType.RT;

                if (isRT)
                {
                    // Preempt the oldest NRT call and admit the incoming RT call into buffer 1
                    var oldestNRTCall = buffer1.Where(c => c.Type == CallType.NRT).OrderBy(c => c.TimeArrived).FirstOrDefault();

                    if (oldestNRTCall != null)
                    {
                        buffer1.Remove(oldestNRTCall);
                        buffer1.Add(incomingCall);
                    }
                    else if (buffer2.Count < buffer2Size)
                    {
                        // If there are no NRT calls in buffer 1, admit the incoming RT call into buffer 2
                        buffer2.Add(incomingCall);
                    }
                    else
                    {
                        // Reject the incoming RT call since both buffers are full
                        RTBlockingProb = 1;
                    }
                }
                else
                {
                    if (buffer2.Count < buffer2Size)
                    {
                        // Admit the incoming NRT call into buffer 2
                        buffer2.Add(incomingCall);
                    }
                    else
                    {
                        // Reject the incoming NRT call since both buffers are full
                        NRTBlockingProb = 1;
                    }
                }
            }
        }

        /* void AddCallToQueue(Call call)
         {
             if (buffer1.Count < buffer1Size)
             {
                 // buffer 1 is not full, insert call into the queue
                 buffer1.Enqueue(call);
             }
             else
             {
                 // buffer 1 is full
                 if (call.Type == CallType.RT)
                 {
                     // incoming call is RT, preempt the oldest NRT call and admit the incoming RT call into the buffer
                     if (buffer1.Contains(CallType.NRT))
                     {
                         buffer1.Dequeue(); // remove the oldest NRT call from buffer 1
                         buffer1.Enqueue(callType); // add the incoming RT call to buffer 1
                     }
                 }
                 else if (callType == "NRT")
                 {
                     // incoming call is NRT
                     if (!buffer1.Contains("NRT"))
                     {
                         // no NRT call in the buffer, reject NRT call
                         Console.WriteLine("Rejected NRT call.");
                     }
                     else if (buffer2.Count < buffer2Size)
                     {
                         // buffer 2 is not full, insert call into buffer 2
                         buffer2.Enqueue(callType);
                     }
                     else
                     {
                         // buffer 2 is full, reject NRT call
                         Console.WriteLine("Rejected NRT call.");
                     }
                 }
             }
         }*/
        // Define the function to allocate resources to calls in the buffer
        int availableChannels = 10; // Set the number of available channels

        void AllocateResources(Queue<string> rtCalls, Queue<string> nrtCalls)
        {
            int rtCallsCount = rtCalls.Count;
            int nrtCallsCount = nrtCalls.Count;

            if (availableChannels > 0)
            {
                if (rtCallsCount > 0 && nrtCallsCount > 0)
                {
                    // Allocate channels to 2 RT calls and then 1 NRT call
                    int rtChannels = Math.Min(rtCallsCount, 2); // Allocate channels to at most 2 RT calls
                    int nrtChannels = Math.Min(1, availableChannels - rtChannels); // Allocate channels to at most 1 NRT call

                    AllocateChannels(rtCalls, rtChannels, "RT");
                    AllocateChannels(nrtCalls, nrtChannels, "NRT");
                }
                else if (rtCallsCount > 0)
                {
                    // Allocate channels to RT calls
                    int rtChannels = Math.Min(rtCallsCount, availableChannels);
                    AllocateChannels(rtCalls, rtChannels, "RT");
                }
                else if (nrtCallsCount > 0)
                {
                    // Allocate channels to NRT calls
                    int nrtChannels = Math.Min(nrtCallsCount, availableChannels);
                    AllocateChannels(nrtCalls, nrtChannels, "NRT");
                }
            }
        }


        void AllocateChannels(Queue<string> calls, int channels, string callType)
        {
            for (int i = 0; i < channels; i++)
            {
                string call = calls.Dequeue();
                Console.WriteLine($"Allocating channel to {callType} call: {call}");
                availableChannels--;
            }
        }
        public void AllocateResourcesToCalls(List<Call> rtCalls, List<Call> nrtCalls, int numChannels)
        {
            // Check if channels are available
            if (numChannels > 0)
            {
                // Check if there are both RT and NRT calls in the buffer
                if (rtCalls.Count > 0 && nrtCalls.Count > 0)
                {
                    // Allocate resources to 2 RT calls and 1 NRT call
                    for (int i = 0; i < 2 && i < rtCalls.Count; i++)
                    {
                        rtCalls[i].AllocateResources(numChannels);
                        numChannels -= rtCalls[i].NumResourceBlocks;
                    }
                    if (numChannels > 0 && nrtCalls.Count > 0)
                    {
                        nrtCalls[0].AllocateResources(numChannels);
                    }
                }
                // Check if there are only RT calls in the buffer
                else if (rtCalls.Count > 0 && nrtCalls.Count == 0)
                {
                    // Allocate resources to RT calls
                    foreach (Call call in rtCalls)
                    {
                        if (numChannels > 0)
                        {
                            call.AllocateResources(numChannels);
                            numChannels -= call.NumResourceBlocks;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                // Check if there are only NRT calls in the buffer
                else if (rtCalls.Count == 0 && nrtCalls.Count > 0)
                {
                    // Allocate resources to NRT calls
                    foreach (Call call in nrtCalls)
                    {
                        if (numChannels > 0)
                        {
                            call.AllocateResources(numChannels);
                            numChannels -= call.NumResourceBlocks;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }


    }
}
