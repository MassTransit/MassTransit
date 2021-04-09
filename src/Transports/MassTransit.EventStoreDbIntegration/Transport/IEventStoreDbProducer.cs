using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbProducer :
        ISendObserverConnector
    {
        /// <summary>
        /// Produces a message to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(T message, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces messages to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="messages">The message values</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces a message to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(T message, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces messages to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="messages">The message values</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(IEnumerable<T> messages, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces a message to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(object values, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces a messages to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces a message to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(object values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Produces a messages to the configured EventStoreDB stream.
        /// </summary>
        /// <param name="values">The message values</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        Task Produce<T>(IEnumerable<object> values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class;
    }
}
