namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public interface ISubscriptionConfigurator :
        IEndpointEntityConfigurator,
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
        Filter Filter { set; }

        /// <summary>
        /// Specify a rule for the subscription
        /// </summary>
        RuleDescription Rule { set; }

        SubscriptionDescription GetSubscriptionDescription();
    }
}
