namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public static class PublishContextExecuteExtensions
    {
        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish<T>(this IPublishEndpoint endpoint, T message, Action<PublishContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Publish(message, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish<T>(this IPublishEndpoint endpoint, T message, Func<PublishContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Publish(message, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish(this IPublishEndpoint endpoint, object message, Action<PublishContext> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Publish(message, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish(this IPublishEndpoint endpoint, object message, Func<PublishContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Publish(message, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish(this IPublishEndpoint endpoint, object message, Type messageType, Action<PublishContext> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Publish(message, messageType, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish(this IPublishEndpoint endpoint, object message, Type messageType, Func<PublishContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Publish(message, messageType, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish<T>(this IPublishEndpoint endpoint, object values, Action<PublishContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Publish(values, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Publish a message, using a callback to modify the publish context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Publish<T>(this IPublishEndpoint endpoint, object values, Func<PublishContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Publish(values, Pipe.ExecuteAsync(callback), cancellationToken);
        }
    }
}
