namespace MassTransit
{
    using Azure.Messaging.ServiceBus.Administration;


    public interface IServiceBusSubscriptionConfigurator :
        IServiceBusEndpointEntityConfigurator,
        ISpecification
    {
        /// <summary>
        /// The path of the subscription's topic
        /// </summary>
        string TopicPath { get; }

        /// <summary>
        /// The subscription name, unique per topic
        /// </summary>
        string SubscriptionName { get; }

        /// <summary>
        /// Sets the path where messages are forwarded to
        /// </summary>
        string ForwardTo { set; }

        /// <summary>
        /// Move messages to the dead letter queue on filter evaluation exception
        /// </summary>
        bool? EnableDeadLetteringOnFilterEvaluationExceptions { set; }

        /// <summary>
        /// Specify the filter for the subscription
        /// </summary>
        RuleFilter Filter { set; }

        /// <summary>
        /// Specify a rule for the subscription
        /// </summary>
        CreateRuleOptions Rule { set; }

        CreateSubscriptionOptions GetCreateSubscriptionOptions();
    }
}
