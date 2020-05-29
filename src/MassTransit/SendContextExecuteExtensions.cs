namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public static class SendContextExecuteExtensions
    {
        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send<T>(this ISendEndpoint endpoint, T message, Action<SendContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(message, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send<T>(this ISendEndpoint endpoint, T message, Func<SendContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(message, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Action<SendContext> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Func<SendContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Type messageType, Action<SendContext> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, messageType, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="messageType">The message type to send the object as</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Type messageType, Func<SendContext, Task> callback,
            CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, messageType, Pipe.ExecuteAsync(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send<T>(this ISendEndpoint endpoint, object values, Action<SendContext<T>> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(values, Pipe.Execute(callback), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="values">The values that map to the object</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send<T>(this ISendEndpoint endpoint, object values, Func<SendContext<T>, Task> callback,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(values, Pipe.ExecuteAsync(callback), cancellationToken);
        }
    }
}
