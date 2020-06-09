namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IEventHubProducer :
        ISendObserverConnector
    {
        /// <summary>
        /// Produces a message to the configured EventHub topic.
        /// </summary>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(TValue value, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a messages to the configured EventHub topic.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(IEnumerable<TValue> values, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a message to the configured EventHub topic.
        /// </summary>
        /// <param name="value">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(TValue value, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces messages to the configured EventHub topic.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(IEnumerable<TValue> values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a message to the configured Kafka topic.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(object values, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a messages to the configured EventHub topic.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a message to the configured EventHub topic.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(object values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class;

        /// <summary>
        /// Produces a messages to the configured EventHub topic.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        Task Produce<TValue>(IEnumerable<object> values, IPipe<EventHubSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
            where TValue : class;
    }
}
