using Microsoft.CodeAnalysis.Rename;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Runtime.ConstrainedExecution;

namespace DualBuffer.Models.Enums
{
    public class Call
    {

        public Call()
        {
          
        }

        public Call(CallType callType)
        {
            Type = callType;
        

        }

        public Call( CallType callType, double callDuration, double signalToNoiseRatio, double requiredBandwidth, int totalChannels, int allocatedChannels)
        {
            Type = callType;
            callDuration = callDuration;
            signalToNoiseRatio = signalToNoiseRatio;
            requiredBandwidth = requiredBandwidth;
            totalChannels = totalChannels;
            allocatedChannels = allocatedChannels;

        }
        public int Id { get; set; }
        public CallType Type { get; set; }
        public DateTime TimeArrived { get; set; }
        public int NumResourceBlocks { get; set; }
        public CallStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public double callDuration { get; set; }
        public double signalToNoiseRatio { get; set; }
        public double requiredBandwidth { get; set; }
        public int totalChannels { get; set; }
        
        public int allocatedChannels { get; set; }
        public bool IsBlocked { get; internal set; }
        public int WaitingTime { get; internal set; }

        public void AllocateResources(int numChannels)
        {
            int blocksToAllocate = Math.Min(numChannels, NumResourceBlocks);
            NumResourceBlocks -= blocksToAllocate;
            Console.WriteLine($"Allocating {blocksToAllocate} resource blocks to the call.");
        }
    }
   

}