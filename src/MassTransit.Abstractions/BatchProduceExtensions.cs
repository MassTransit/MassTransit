namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BatchProduceExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch<T>(this ISendEndpoint endpoint, IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch<T>(this ISendEndpoint endpoint, IEnumerable<T> messages, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, messageType, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Send(x, messageType, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task PublishBatch<T>(this IPublishEndpoint endpoint, IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, cancellationToken)));
        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch<T>(this IPublishEndpoint endpoint, IEnumerable<T> messages, IPipe<PublishContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, cancellationToken)));
        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, messageType, cancellationToken)));
        }

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, messageType, pipe, cancellationToken)));
        }
    }
}
