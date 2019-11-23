namespace MassTransit.AzureServiceBusTransport.Topology.Configuration
{
    using System;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


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
        /// Create the queueDescription for the configuration
        /// </summary>
        /// <returns></returns>
        QueueDescription GetQueueDescription();

        /// <summary>
        /// Returns the address of the queue
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        Uri GetQueueAddress(Uri hostAddress);
    }
}
