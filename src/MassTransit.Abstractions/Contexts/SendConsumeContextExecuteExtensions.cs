namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public static class SendConsumeContextExecuteExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, Action<SendContext<T>> callback)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, Func<SendContext<T>, Task> callback)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Action<SendContext> callback)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Func<SendContext, Task> callback)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType, Action<SendContext> callback)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType, Func<SendContext, Task> callback)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, Action<SendContext<T>> callback)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="callback">The callback for the send context</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, Func<SendContext<T>, Task> callback)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, callback.ToPipe(), context.CancellationToken).ConfigureAwait(false);
        }
    }
}
