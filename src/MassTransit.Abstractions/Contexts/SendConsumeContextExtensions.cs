namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public static class SendConsumeContextExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext<T>> pipe)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext> pipe)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType, IPipe<SendContext> pipe)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, IPipe<SendContext> pipe)
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext<T>> pipe)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(values, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext> pipe)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, pipe, context.CancellationToken).ConfigureAwait(false);
        }
    }
}
