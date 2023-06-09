﻿using DualBuffer.Models.Enums;

namespace DualBuffer.Services
{
    public interface INetworkService
    {
        public bool AcceptRequest(double callDuration, double signalToNoiseRatio, double requiredBandwidth, int totalChannels, int allocatedChannels);


        public bool AcceptRequestIntoBuffer(Call incomingCall, TimeSpan waitingTime);


        public void AllocateResourcesToCalls(List<Call> rtCalls, List<Call> nrtCalls, int numChannels);

        public List<Call> GetRTCalls();

        public List<Call> GetNRTCalls();


        public Call AddCall(Call call);

        public List<Call> ListCalls();

        public double CalculateAverageWaitingTime();

        public double CalculateSystemUtilization();

        public double CalculatePacketLossRate();

        public double CalculateThroughput();

        public double CalculateFairnessIndex();

        public double CalculateRTBlockingProbability();

        public double CalculateNRTBlockingProbability();

        public void DeleteCall(int id);

    }
}
