namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Middleware;
    using Serialization;


    public static class ForwardExtensions
    {
        public static async Task Forward<T>(this ConsumeContext<T> context, Uri address)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint).ConfigureAwait(false);
        }

        public static async Task Forward<T>(this ConsumeContext<T> context, Uri address, IPipe<SendContext<T>> pipe)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint, pipe).ConfigureAwait(false);
        }

        public static Task Forward<T>(this ConsumeContext<T> context, ISendEndpoint endpoint)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            var messagePipe = new ForwardMessagePipe<T>(context);

            return endpoint.Send(context.Message, messagePipe, context.CancellationToken);
        }

        public static Task Forward<T>(this ConsumeContext<T> context, ISendEndpoint endpoint, IPipe<SendContext<T>> pipe)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            var messagePipe = new ForwardMessagePipe<T>(context, pipe);

            return endpoint.Send(context.Message, messagePipe, context.CancellationToken);
        }

        public static async Task Forward<T>(this ConsumeContext context, Uri address, T message)
            where T : class
        {
            var endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint, message).ConfigureAwait(false);
        }

        /// <summary>
        /// Forward the message to another consumer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="message"></param>
        public static Task Forward<T>(this ConsumeContext context, ISendEndpoint endpoint, T message)
            where T : class
        {
            void AddForwarderAddress(ConsumeContext consumeContext, SendContext sendContext)
            {
                var forwarderAddress = consumeContext.ReceiveContext.InputAddress ?? consumeContext.DestinationAddress;
                if (forwarderAddress != null && forwarderAddress != context.DestinationAddress)
                    sendContext.Headers.Set(MessageHeaders.ForwarderAddress, forwarderAddress.ToString());
            }

            return endpoint.Send(message, new CopyContextPipe(context, AddForwarderAddress));
        }
    }
}
