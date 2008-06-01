namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Heartbeat
    {
        public readonly int TimeBetweenBeatsInSeconds;
        public readonly Uri EndpointAddress;


        public Heartbeat(int timeBetweenBeatsInSeconds, Uri endpointAddress)
        {
            TimeBetweenBeatsInSeconds = timeBetweenBeatsInSeconds;
            EndpointAddress = endpointAddress;
        }
    }
}