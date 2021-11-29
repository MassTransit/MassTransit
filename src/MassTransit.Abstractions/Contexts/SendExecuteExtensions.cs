namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class SendExecuteExtensions
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
        public static Task Send<T>(this ISendEndpoint endpoint, T message, Action<SendContext<T>> callback, CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(message, callback.ToPipe(), cancellationToken);
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
        public static Task Send<T>(this ISendEndpoint endpoint, T message, Func<SendContext<T>, Task> callback, CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(message, callback.ToPipe(), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Action<SendContext> callback, CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, callback.ToPipe(), cancellationToken);
        }

        /// <summary>
        /// Send a message, using a callback to modify the send context instead of building a pipe from scratch
        /// </summary>
        /// <param name="endpoint">The endpoint to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <param name="cancellationToken">To cancel the send from happening</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task Send(this ISendEndpoint endpoint, object message, Func<SendContext, Task> callback, CancellationToken cancellationToken = default)
        {
            return endpoint.Send(message, callback.ToPipe(), cancellationToken);
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
            return endpoint.Send(message, messageType, callback.ToPipe(), cancellationToken);
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
            return endpoint.Send(message, messageType, callback.ToPipe(), cancellationToken);
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
        public static Task Send<T>(this ISendEndpoint endpoint, object values, Action<SendContext<T>> callback, CancellationToken cancellationToken = default)
            where T : class
        {
            return endpoint.Send(values, callback.ToPipe(), cancellationToken);
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
            return endpoint.Send(values, callback.ToPipe(), cancellationToken);
        }

        public static IPipe<SendContext<T>> ToPipe<T>(this Action<SendContext<T>> callback)
            where T : class
        {
            return new SendContextPipe<T>(callback);
        }

        public static IPipe<SendContext<T>> ToPipe<T>(this Func<SendContext<T>, Task> callback)
            where T : class
        {
            return new SendContextAsyncPipe<T>(callback);
        }

        public static IPipe<SendContext> ToPipe(this Action<SendContext> callback)
        {
            return new SendContextPipe(callback);
        }

        public static IPipe<SendContext> ToPipe(this Func<SendContext, Task> callback)
        {
            return new SendContextAsyncPipe(callback);
        }


        class SendContextPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly Action<SendContext<T>> _callback;

            public SendContextPipe(Action<SendContext<T>> callback)
            {
                _callback = callback;
            }

            public Task Send(SendContext<T> context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class SendContextAsyncPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly Func<SendContext<T>, Task> _callback;

            public SendContextAsyncPipe(Func<SendContext<T>, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(SendContext<T> context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class SendContextPipe :
            IPipe<SendContext>
        {
            readonly Action<SendContext> _callback;

            public SendContextPipe(Action<SendContext> callback)
            {
                _callback = callback;
            }

            public Task Send(SendContext context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class SendContextAsyncPipe :
            IPipe<SendContext>
        {
            readonly Func<SendContext, Task> _callback;

            public SendContextAsyncPipe(Func<SendContext, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(SendContext context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }
    }
}
