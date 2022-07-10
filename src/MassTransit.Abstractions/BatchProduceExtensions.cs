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
        /// Send a message batch
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
        /// Send a message batch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch<T>(this ISendEndpoint endpoint, IEnumerable<T> messages, Action<SendContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IPipe<SendContext<T>> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Send(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message batch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch<T>(this ISendEndpoint endpoint, IEnumerable<T> messages, Func<SendContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IPipe<SendContext<T>> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Send(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message batch
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
        /// Send a message batch
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
        /// Send a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Action<SendContext> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<SendContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Send(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Func<SendContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<SendContext> pipe = callback.ToPipe();

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
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, Action<SendContext> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<SendContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Send(x, messageType, pipe, cancellationToken)));
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task SendBatch(this ISendEndpoint endpoint, IEnumerable<object> messages, Type messageType, Func<SendContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<SendContext> pipe = callback.ToPipe();

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
        /// Publish a message batch
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
        /// Publish a message batch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch<T>(this IPublishEndpoint endpoint, IEnumerable<T> messages, Action<PublishContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IPipe<PublishContext<T>> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message batch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch<T>(this IPublishEndpoint endpoint, IEnumerable<T> messages, Func<PublishContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IPipe<PublishContext<T>> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message batch
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
        /// Publish a message batch
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
        /// Publish a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Action<PublishContext> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<PublishContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Func<PublishContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<PublishContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message batch
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
        /// Publish a message batch
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

        /// <summary>
        /// Publish a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, Action<PublishContext> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<PublishContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, messageType, pipe, cancellationToken)));
        }

        /// <summary>
        /// Publish a message batch
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="messages"></param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the publish context</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Publish is acknowledged by the broker</returns>
        public static Task PublishBatch(this IPublishEndpoint endpoint, IEnumerable<object> messages, Type messageType, Func<PublishContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            IPipe<PublishContext> pipe = callback.ToPipe();

            return Task.WhenAll(messages.Select(x => endpoint.Publish(x, messageType, pipe, cancellationToken)));
        }
    }
}
