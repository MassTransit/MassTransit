namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class DownEndpoint
    {
        public readonly IEndpoint Endpoint;


        public DownEndpoint(IEndpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}