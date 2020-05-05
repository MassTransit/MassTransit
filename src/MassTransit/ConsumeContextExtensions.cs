namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    public static class ConsumeContextExtensions
    {
        /// <summary>
        /// Returns the endpoint for a fault, either directly to the requester, or published
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        public static Task<ISendEndpoint> GetFaultEndpoint<T>(this ConsumeContext context)
            where T : class
        {
            var destinationAddress = context.FaultAddress ?? context.ResponseAddress;

            return GetEndpoint<Fault<T>>(context.ReceiveContext, context, destinationAddress, context.RequestId);
        }

        /// <summary>
        /// Returns the endpoint for a fault, either directly to the requester, or published
        /// </summary>
        /// <param name="context"></param>
        /// <param name="faultAddress"></param>
        /// <param name="requestId"></param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns></returns>
        public static Task<ISendEndpoint> GetFaultEndpoint<T>(this ConsumeContext context, Uri faultAddress, Guid? requestId = default)
            where T : class
        {
            var destinationAddress = faultAddress ?? context.FaultAddress ?? context.ResponseAddress;

            return GetEndpoint<T>(context.ReceiveContext, context, destinationAddress, requestId ?? context.RequestId);
        }

        /// <summary>
        /// Returns the endpoint for a receive fault, either directly to the requester, or published
        /// </summary>
        /// <param name="context"></param>
        /// <param name="consumeContext"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static Task<ISendEndpoint> GetReceiveFaultEndpoint(this ReceiveContext context, ConsumeContext consumeContext, Guid? requestId)
        {
            var destinationAddress = consumeContext?.FaultAddress ?? consumeContext?.ResponseAddress;

            return GetEndpoint<ReceiveFault>(context, consumeContext, destinationAddress, requestId);
        }

        /// <summary>
        /// Returns the endpoint for a response, either directly to the requester, or published
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<ISendEndpoint> GetResponseEndpoint<T>(this ConsumeContext context)
            where T : class
        {
            return GetEndpoint<T>(context.ReceiveContext, context, context.ResponseAddress, context.RequestId);
        }

        /// <summary>
        /// Returns the endpoint for a response, either directly to the requester, or published
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseAddress"></param>
        /// <param name="requestId"></param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns></returns>
        public static Task<ISendEndpoint> GetResponseEndpoint<T>(this ConsumeContext context, Uri responseAddress, Guid? requestId = default)
            where T : class
        {
            return GetEndpoint<T>(context.ReceiveContext, context, responseAddress ?? context.ResponseAddress, requestId ?? context.RequestId);
        }

        /// <summary>
        /// Returns the endpoint for a response, either directly to the requester, or published
        /// </summary>
        /// <param name="receiveContext"></param>
        /// <param name="consumeContext"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="requestId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static Task<ISendEndpoint> GetEndpoint<T>(ReceiveContext receiveContext, ConsumeContext consumeContext, Uri destinationAddress, Guid? requestId)
            where T : class
        {
            if (destinationAddress != null && consumeContext != null)
            {
                Task<ISendEndpoint> sendEndpointTask = receiveContext.SendEndpointProvider.GetSendEndpoint(destinationAddress);
                if (sendEndpointTask.IsCompletedSuccessfully())
                    return Task.FromResult<ISendEndpoint>(new ConsumeSendEndpoint(sendEndpointTask.Result, consumeContext, requestId));

                async Task<ISendEndpoint> GetResponseEndpointAsync()
                {
                    var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                    return new ConsumeSendEndpoint(sendEndpoint, consumeContext, requestId);
                }

                return GetResponseEndpointAsync();
            }

            Task<ISendEndpoint> publishSendEndpointTask = receiveContext.PublishEndpointProvider.GetPublishSendEndpoint<T>();
            if (publishSendEndpointTask.IsCompletedSuccessfully())
                return consumeContext != null
                    ? Task.FromResult<ISendEndpoint>(new ConsumeSendEndpoint(publishSendEndpointTask.Result, consumeContext, requestId))
                    : publishSendEndpointTask;

            async Task<ISendEndpoint> GetPublishSendEndpointAsync()
            {
                var publishSendEndpoint = await publishSendEndpointTask.ConfigureAwait(false);

                return consumeContext != null
                    ? new ConsumeSendEndpoint(publishSendEndpoint, consumeContext, requestId)
                    : publishSendEndpoint;
            }

            return GetPublishSendEndpointAsync();
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
