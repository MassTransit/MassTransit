namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;

    [Serializable]
    public class Suspect
    {
        public Uri EndpointUri;
    }
}