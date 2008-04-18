namespace MassTransit.Patterns.FaultDetection
{
    using System;
    using MassTransit.ServiceBus;

    public class MonitorInfo
    {
        public IEndpoint Endpoint;
        public DateTime LastHeartbeat;

        public MonitorInfo(IEndpoint ep, DateTime lastBeat)
        {
            Endpoint = ep;
            LastHeartbeat = lastBeat;
        }
    }
}
