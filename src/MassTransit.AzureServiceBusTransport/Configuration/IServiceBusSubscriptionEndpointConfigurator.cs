namespace MassTransit.AzureServiceBusTransport
{
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusSubscriptionEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IServiceBusEndpointConfigurator
    {
        /// <summary>
        /// Specify the filter for the subscription
        /// </summary>
        Filter Filter { set; }

        /// <summary>
        /// Specify a rule for the subscription
        /// </summary>
        RuleDescription Rule { set; }
    }
}
