namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;


    public interface IQueueConfigurator :
        IMessageEntityConfigurator,
        IEndpointEntityConfigurator,
        ISpecification
    {
        /// <summary>
        /// Move messages to the dead letter queue on filter evaluation exception
        /// </summary>
        bool? EnableDeadLetteringOnFilterEvaluationExceptions { set; }

        /// <summary>
        /// Sets the path where messages are forwarded to
        /// </summary>
        string ForwardTo { set; }

        /// <summary>
        /// Create the CreateQueueOptions for the configuration
        /// </summary>
        /// <returns></returns>
        CreateQueueOptions GetCreateQueueOptions();
    }
}
