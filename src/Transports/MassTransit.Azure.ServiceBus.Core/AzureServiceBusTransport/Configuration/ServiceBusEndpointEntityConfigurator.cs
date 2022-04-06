namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;


    public class ServiceBusEndpointEntityConfigurator :
        ServiceBusEntityConfigurator,
        IServiceBusEndpointEntityConfigurator
    {
        public bool? EnableDeadLetteringOnMessageExpiration { get; set; }

        public string ForwardDeadLetteredMessagesTo { get; set; }

        public TimeSpan? LockDuration { get; set; }

        public int? MaxDeliveryCount { get; set; }

        public bool? RequiresSession { get; set; }

        public int? MaxConcurrentCallsPerSession { get; set; }
    }
}
