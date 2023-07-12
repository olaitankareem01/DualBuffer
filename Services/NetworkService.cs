using DualBuffer.Models.Enums;
using DualBuffer.Repositories;
using System;

namespace DualBuffer.Services
{
    public class NetworkService: INetworkService
    {
        private readonly INetworkRepository _networkRepository;
        public NetworkService(INetworkRepository networkRepository) 
        {
            _networkRepository = networkRepository;
            StartCallRemovalTimer();
        }

        private static readonly double removalIntervalMinutes = 60; 
        private Timer removalTimer;

        private const int Buffer1Size = 1;
        private const int Buffer2Size = 2;
        private int RTBlockingProb;
        private int NRTBlockingProb;

        private static Queue<Call> buffer1 = new Queue<Call>();
        private static Queue<Call> buffer2 = new Queue<Call>();
        private int availableChannels = 5;

        public bool AcceptRequest(double callDuration, double signalToNoiseRatio, double requiredBandwidth, int totalChannels, int allocatedChannels)
        {
            double bandwidthAllocation = callDuration * Math.Log(1 + signalToNoiseRatio);
            double totalBandwidthAllocation = bandwidthAllocation;
            double availableBandwidth = 0;

            for (int i = 2; i <= allocatedChannels; i++)
            {
                double channelAllocation = 1;
                double delta = channelAllocation * bandwidthAllocation;
                totalBandwidthAllocation += delta;
            }

            availableBandwidth = totalBandwidthAllocation <= totalChannels - allocatedChannels ? totalBandwidthAllocation : 0;

            return availableBandwidth + bandwidthAllocation <= totalChannels - allocatedChannels;
        }

        public bool AcceptRequestIntoBuffer(Call incomingCall, TimeSpan waitingTime)
        {

            incomingCall.TimeArrived = DateTime.Now;
            incomingCall.ExpiresAt  = DateTime.Now.Add(waitingTime);

            if (buffer1.Count < Buffer1Size)
            {
                buffer1.Enqueue(incomingCall);
                return true;
            }
            else
            {
                bool isRT = incomingCall.Type == CallType.RT;

                if (isRT)
                {
                    var oldestNRTCall = buffer1.Where(c => c.Type == CallType.NRT)
                                        .OrderBy(c => c.TimeArrived)
                                        .FirstOrDefault();

                    if (oldestNRTCall != null)
                    {
                        buffer1 = new Queue<Call>(buffer1.Where(c => c != oldestNRTCall));
                        buffer1.Enqueue(incomingCall);
                        return true;
                    }
                    else if (buffer2.Count < Buffer2Size)
                    {
                     
                        buffer2.Enqueue(incomingCall);
                        return true;
                    }
                    else
                    {
                        incomingCall.IsBlocked = true;
                        incomingCall.Status = CallStatus.Failed;
                        RTBlockingProb = 1;
                        return false; // Reject the incoming RT call since both buffers are full
                    }
                }
                else
                {
                    if (buffer2.Count < Buffer2Size)
                    {
                        buffer2.Enqueue(incomingCall);
                    }
                    else
                    {
                        // Reject the incoming NRT call since both buffers are full
                        incomingCall.IsBlocked = true;
                        incomingCall.Status = CallStatus.Failed;
                        NRTBlockingProb = 1;
                        return false;// Reject the incoming NRT call since buffer 2 is full
                    }
                }
            }
            return false;
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
                    int rtCallsToAllocate = Math.Min(rtCalls.Count, 2);
                    int nrtCallsToAllocate = 1;

                    // Allocate resources to RT calls
                    for (int i = 0; i < rtCallsToAllocate; i++)
                    {
                        Call rtCall = rtCalls[i];
                        if (rtCall.NumResourceBlocks <= numChannels)
                        {
                            rtCall.AllocateResources(numChannels);
                            numChannels -= rtCall.NumResourceBlocks;
                        }
                        else
                        {
                            Console.WriteLine($"Insufficient channels to allocate resources for RT call: {rtCall.Id}");
                        }
                    }

                    // Allocate resources to NRT call
                    if (numChannels > 0 && nrtCallsToAllocate <= nrtCalls.Count)
                    {
                        Call nrtCall = nrtCalls[0];
                        if (nrtCall.NumResourceBlocks <= numChannels)
                        {
                            nrtCall.AllocateResources(numChannels);
                        }
                        else
                        {
                            Console.WriteLine($"Insufficient channels to allocate resources for NRT call: {nrtCall.Id}");
                        }
                    }
                }
                // Check if there are only RT calls in the buffer
                else if (rtCalls.Count > 0 && nrtCalls.Count == 0)
                {
                    // Allocate resources to RT calls
                    foreach (Call rtCall in rtCalls)
                    {
                        if (numChannels > 0)
                        {
                            if (rtCall.NumResourceBlocks <= numChannels)
                            {
                                rtCall.AllocateResources(numChannels);
                                numChannels -= rtCall.NumResourceBlocks;
                            }
                            else
                            {
                                Console.WriteLine($"Insufficient channels to allocate resources for RT call: {rtCall.Id}");
                            }
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
                    foreach (Call nrtCall in nrtCalls)
                    {
                        if (numChannels > 0)
                        {
                            if (nrtCall.NumResourceBlocks <= numChannels)
                            {
                                nrtCall.AllocateResources(numChannels);
                                numChannels -= nrtCall.NumResourceBlocks;
                            }
                            else
                            {
                                Console.WriteLine($"Insufficient channels to allocate resources for NRT call: {nrtCall.Id}");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No available channels to allocate resources.");
            }
        }


        public List<Call> GetRTCalls()
        {
            return buffer1.Where(call => call.Type == CallType.RT).ToList();
        }

        public List<Call> GetNRTCalls()
        {
            return buffer1.Where(call => call.Type == CallType.NRT).ToList();
        }

        public  Call AddCall(Call call)
        {
           var callAdded =  _networkRepository.AddAsync(call);
            return callAdded;
        }


        public void StartCallRemovalTimer()
        {
            TimeSpan interval = TimeSpan.FromMinutes(removalIntervalMinutes);
            removalTimer = new Timer(RemoveExpiredCalls, null, interval, interval);
        }

        public void StopCallRemovalTimer()
        {
            removalTimer?.Dispose();
            removalTimer = null;
        }

        public void RemoveExpiredCalls(object state)
        {
            DateTime currentTime = DateTime.Now;

            // Remove expired calls from buffer 1
            int buffer1Count = buffer1.Count;
            for (int i = 0; i < buffer1Count; i++)
            {
                Call call = buffer1.Peek();
                if (call.ExpiresAt <= currentTime)
                {
                    var callFound = _networkRepository.GetAsync(call.Id);
                    if(callFound != null)
                    {
                        callFound.Status = CallStatus.Expired;
                        _networkRepository.UpdateAsync(callFound);
                    }
                    buffer1.Dequeue();
                }
                else
                {
                    buffer1.Enqueue(call); // Re-enqueue the non-expired call
                }
            }

            // Remove expired calls from buffer 2
            int buffer2Count = buffer2.Count;
            for (int i = 0; i < buffer2Count; i++)
            {
                Call call = buffer2.Peek();
                if (call.ExpiresAt <= currentTime)
                {
                    buffer2.Dequeue();
                }
                else
                {
                    buffer2.Enqueue(call); // Re-enqueue the non-expired call
                }
            }
        }

        public double CalculateRTBlockingProbability()
        {
            var calls =  _networkRepository.GetAllAsync();
            int totalRTCalls = calls.Count(c => c.Type == CallType.RT);
            int blockedRTCalls = calls.Count(c => c.Type == CallType.RT && c.IsBlocked);

            if (totalRTCalls > 0)
            {
                return (double)blockedRTCalls / totalRTCalls;
            }
            else
            {
                return 0;
            }
        }

        // Calculate the NRT Blocking Probability
        public double CalculateNRTBlockingProbability()
        {
            var calls = _networkRepository.GetAllAsync();
            int totalNRTCalls =  calls.Count(c => c.Type == CallType.NRT);
            int blockedNRTCalls = calls.Count(c => c.Type == CallType.NRT && c.IsBlocked);

            if (totalNRTCalls > 0)
            {
                return (double)blockedNRTCalls / totalNRTCalls;
            }
            else
            {
                return 0;
            }
        }

    

        public double CalculateFairnessIndex()
        {

            var calls = _networkRepository.GetAllAsync();
            double rtCallsSum = calls.Sum(c => c.Type == CallType.RT ? c.NumResourceBlocks : 0);
            double nrtCallsSum = calls.Sum(c => c.Type == CallType.NRT ? c.NumResourceBlocks : 0);

            double rtCallsSquaredSum = calls.Sum(c => c.Type == CallType.RT ? Math.Pow(c.NumResourceBlocks, 2) : 0);
            double nrtCallsSquaredSum = calls.Sum(c => c.Type == CallType.NRT ? Math.Pow(c.NumResourceBlocks, 2) : 0);

            double rtCallsCount = calls.Count(c => c.Type == CallType.RT);
            double nrtCallsCount = calls.Count(c => c.Type == CallType.NRT);

            double rtFairness = Math.Pow(rtCallsSum, 2) / (rtCallsCount * rtCallsSquaredSum);
            double nrtFairness = Math.Pow(nrtCallsSum, 2) / (nrtCallsCount * nrtCallsSquaredSum);

            return Math.Min(rtFairness, nrtFairness);
        }

        // Calculate throughput
        public double CalculateThroughput()
        {
            var calls = _networkRepository.GetAllAsync();
            double rtCallsSum = calls.Sum(c => c.Type == CallType.RT ? c.NumResourceBlocks : 0);
            double nrtCallsSum = calls.Sum(c => c.Type == CallType.NRT ? c.NumResourceBlocks : 0);

            double totalCapacity = availableChannels * Buffer1Size;

            return (rtCallsSum + nrtCallsSum) / totalCapacity;
        }

        // Calculate packet loss rate
        public double CalculatePacketLossRate()
        {
            var calls = _networkRepository.GetAllAsync();
            int blockedRTCalls = calls.Count(c => c.Type == CallType.RT && c.IsBlocked);
            int blockedNRTCalls = calls.Count(c => c.Type == CallType.NRT && c.IsBlocked);

            int totalCalls = buffer1.Count + buffer2.Count;

            if (totalCalls > 0)
            {
                return (double)(blockedRTCalls + blockedNRTCalls) / totalCalls;
            }
            else
            {
                return 0;
            }
        }

        // Calculate system utilization
        public double CalculateSystemUtilization()
        {
            var calls = _networkRepository.GetAllAsync();
            int occupiedChannels = calls.Sum(c => c.NumResourceBlocks) + calls.Sum(c => c.NumResourceBlocks);

            return (double)occupiedChannels / availableChannels;
        }

        // Calculate average waiting time
        public double CalculateAverageWaitingTime()
        {
            double totalWaitingTime = buffer1.Sum(c => c.WaitingTime) + buffer2.Sum(c => c.WaitingTime);
            int totalCalls = buffer1.Count + buffer2.Count;

            if (totalCalls > 0)
            {
                return totalWaitingTime / totalCalls;
            }
            else
            {
                return 0;
            }
        }


        public List<Call> ListCalls()
        {
            return _networkRepository.GetAllAsync();
        }

        public void DeleteCall(int id)
        {
           _networkRepository.DeleteAsync(id);
        }

       
    }

}
