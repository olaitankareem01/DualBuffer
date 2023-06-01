namespace DualBuffer.Models.Enums
{
    public class Call
    {
        public int id { get; set; }
        public CallType Type { get; set; }

        public DateTime TimeArrived { get; set; }

        public int NumResourceBlocks { get; set; }

        public CallStatus status { get; set; }

        public DateTime expiresAt { get; set; }

        public  void AllocateResources(int numChannels)
        {
            NumResourceBlocks = numChannels;
            return;
        }
    }
}