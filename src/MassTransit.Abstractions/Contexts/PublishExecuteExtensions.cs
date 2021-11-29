namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class PublishExecuteExtensions
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
            return endpoint.Publish(message, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(message, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(message, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(message, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(message, messageType, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(message, messageType, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(values, callback.ToPipe(), cancellationToken);
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
            return endpoint.Publish(values, callback.ToPipe(), cancellationToken);
        }

        static IPipe<PublishContext<T>> ToPipe<T>(this Action<PublishContext<T>> callback)
            where T : class
        {
            return new PublishContextPipe<T>(callback);
        }

        static IPipe<PublishContext<T>> ToPipe<T>(this Func<PublishContext<T>, Task> callback)
            where T : class
        {
            return new PublishContextAsyncPipe<T>(callback);
        }

        static IPipe<PublishContext> ToPipe(this Action<PublishContext> callback)
        {
            return new PublishContextPipe(callback);
        }

        static IPipe<PublishContext> ToPipe(this Func<PublishContext, Task> callback)
        {
            return new PublishContextAsyncPipe(callback);
        }


        class PublishContextPipe<T> :
            IPipe<PublishContext<T>>
            where T : class
        {
            readonly Action<PublishContext<T>> _callback;

            public PublishContextPipe(Action<PublishContext<T>> callback)
            {
                _callback = callback;
            }

            public Task Send(PublishContext<T> context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class PublishContextAsyncPipe<T> :
            IPipe<PublishContext<T>>
            where T : class
        {
            readonly Func<PublishContext<T>, Task> _callback;

            public PublishContextAsyncPipe(Func<PublishContext<T>, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(PublishContext<T> context)
            {
                return _callback(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class PublishContextPipe :
            IPipe<PublishContext>
        {
            readonly Action<PublishContext> _callback;

            public PublishContextPipe(Action<PublishContext> callback)
            {
                _callback = callback;
            }

            public Task Send(PublishContext context)
            {
                _callback(context);

                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("sendContextCallback");
            }
        }


        class PublishContextAsyncPipe :
            IPipe<PublishContext>
        {
            readonly Func<PublishContext, Task> _callback;

            public PublishContextAsyncPipe(Func<PublishContext, Task> callback)
            {
                _callback = callback;
            }

            public Task Send(PublishContext context)
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
