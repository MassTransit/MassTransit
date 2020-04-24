namespace MassTransit.Azure.ServiceBus.Core.Topology.Configurators
{
    using System;


    public class EndpointEntityConfigurator :
        EntityConfigurator,
        IEndpointEntityConfigurator
    {
        public bool? EnableDeadLetteringOnMessageExpiration { get; set; }

        public string ForwardDeadLetteredMessagesTo { get; set; }

        public TimeSpan? LockDuration { get; set; }

        public int? MaxDeliveryCount { get; set; }

        public bool? RequiresSession { get; set; }
    }
}
