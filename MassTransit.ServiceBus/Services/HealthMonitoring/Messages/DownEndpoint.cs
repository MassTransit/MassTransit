namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class DownEndpoint
    {
        public readonly Uri Endpoint;


        public DownEndpoint(Uri endpoint)
        {
            Endpoint = endpoint;
        }
    }
}