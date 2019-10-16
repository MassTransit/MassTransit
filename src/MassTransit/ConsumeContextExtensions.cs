namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public static class ConsumeContextExtensions
    {
        public static ConsumerConsumeContext<TConsumer, T> PushConsumer<TConsumer, T>(this ConsumeContext<T> context, TConsumer consumer)
            where T : class
            where TConsumer : class
        {
            return new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);
        }

        public static ConsumerConsumeContext<TConsumer, TMessage> PushConsumerScope<TConsumer, TMessage, T>(this ConsumeContext<TMessage> context,
            TConsumer consumer, T scope, params object[] payloads)
            where TMessage : class
            where TConsumer : class
            where T : class
        {
            if (payloads != null && payloads.Length > 0)
                return new ConsumerConsumeContextScope<TConsumer, TMessage>(context, consumer, new object[] {scope}.Concat(payloads));

            return new ConsumerConsumeContextScope<TConsumer, TMessage>(context, consumer, scope);
        }

        public static ConsumeContext<T> CreateScope<T, TScope>(this ConsumeContext<T> context, TScope scope)
            where T : class
            where TScope : class
        {
            return new ConsumeContextScope<T>(context, scope);
        }

        public static Task Forward<T>(this ConsumeContext<T> context, ISendEndpoint endpoint)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            return Forward(context, endpoint, context.Message);
        }

        public static async Task Forward<T>(this ConsumeContext<T> context, Uri address)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var endpoint = await context.GetSendEndpoint(address).ConfigureAwait(false);

            await Forward(context, endpoint, context.Message).ConfigureAwait(false);
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
        /// <param name="endpoint">The endpoint to forward the message tosaq</param>
        /// <param name="message"></param>
        public static Task Forward<T>(this ConsumeContext context, ISendEndpoint endpoint, T message)
            where T : class
        {
            return endpoint.Send(message, CreateCopyContextPipe(context, GetForwardHeaders));
        }

        static IEnumerable<KeyValuePair<string, object>> GetForwardHeaders(ConsumeContext context)
        {
            var inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
            if (inputAddress != null)
                yield return new KeyValuePair<string, object>(MessageHeaders.ForwarderAddress, inputAddress.ToString());
        }

        /// <summary>
        /// Create a send pipe that copies the source message headers to the message being sent
        /// </summary>
        /// <param name="context"></param>
        /// <param name="additionalHeaders">Returns additional headers for the pipe that should be added to the message</param>
        /// <returns></returns>
        public static IPipe<SendContext> CreateCopyContextPipe(this ConsumeContext context,
            Func<ConsumeContext, IEnumerable<KeyValuePair<string, object>>> additionalHeaders)
        {
            return CreateCopyContextPipe(context, (consumeContext, sendContext) =>
            {
                foreach (KeyValuePair<string, object> additionalHeader in additionalHeaders(consumeContext))
                    sendContext.Headers.Set(additionalHeader.Key, additionalHeader.Value);
            });
        }

        /// <summary>
        /// Create a send pipe that copies the source message headers to the message being sent
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback">A callback to modify the send context</param>
        /// <returns></returns>
        public static IPipe<SendContext> CreateCopyContextPipe(this ConsumeContext context, Action<ConsumeContext, SendContext> callback = null)
        {
            return Pipe.Execute<SendContext>(sendContext =>
            {
                sendContext.RequestId = context.RequestId;
                sendContext.CorrelationId = context.CorrelationId;
                sendContext.SourceAddress = context.SourceAddress;
                sendContext.ResponseAddress = context.ResponseAddress;
                sendContext.FaultAddress = context.FaultAddress;
                sendContext.ConversationId = context.ConversationId;
                sendContext.InitiatorId = context.InitiatorId;

                if (context.ExpirationTime.HasValue)
                    sendContext.TimeToLive = context.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

                foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                    sendContext.Headers.Set(header.Key, header.Value);

                callback?.Invoke(context, sendContext);
            });
        }
    }
}
