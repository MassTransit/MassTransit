#nullable enable
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public abstract class BaseConsumeContext :
        PublishEndpoint,
        ConsumeContext
    {
        protected BaseConsumeContext(ReceiveContext receiveContext, SerializerContext serializerContext)
            : base(receiveContext.PublishEndpointProvider)
        {
            ReceiveContext = receiveContext;
            SerializerContext = serializerContext;
        }

        public virtual CancellationToken CancellationToken => ReceiveContext.CancellationToken;

        public abstract bool HasPayloadType(Type payloadType);

        public abstract bool TryGetPayload<T>([NotNullWhen(true)] out T? payload)
            where T : class;

        public abstract T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class;

        public abstract T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class;

        public ReceiveContext ReceiveContext { get; protected set; }

        public SerializerContext SerializerContext { get; }

        public abstract Task ConsumeCompleted { get; }

        public abstract Guid? MessageId { get; }
        public abstract Guid? RequestId { get; }
        public abstract Guid? CorrelationId { get; }
        public abstract Guid? ConversationId { get; }
        public abstract Guid? InitiatorId { get; }
        public abstract DateTime? ExpirationTime { get; }
        public abstract Uri SourceAddress { get; }
        public abstract Uri DestinationAddress { get; }
        public abstract Uri ResponseAddress { get; }
        public abstract Uri FaultAddress { get; }
        public abstract DateTime? SentTime { get; }
        public abstract Headers Headers { get; }
        public abstract HostInfo Host { get; }
        public abstract IEnumerable<string> SupportedMessageTypes { get; }
        public abstract bool HasMessageType(Type messageType);

        public abstract bool TryGetMessage<T>([NotNullWhen(true)] out ConsumeContext<T>? consumeContext)
            where T : class;

        public virtual Task RespondAsync<T>(T message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ConsumeTask(RespondInternal(message));
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            return ConsumeTask(RespondInternal(message, sendPipe));
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            return ConsumeTask(RespondInternal(message, sendPipe));
        }

        public virtual Task RespondAsync(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return ResponseEndpointConverterCache.Respond(this, message, messageType);
        }

        public virtual Task RespondAsync(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return ResponseEndpointConverterCache.Respond(this, message, messageType);
        }

        public virtual Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var messageType = message.GetType();

            return ResponseEndpointConverterCache.Respond(this, message, messageType, sendPipe);
        }

        public virtual Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            return ResponseEndpointConverterCache.Respond(this, message, messageType, sendPipe);
        }

        public virtual Task RespondAsync<T>(object values)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return ConsumeTask(RespondInternal<T>(values));
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return ConsumeTask(RespondInternal(values, sendPipe));
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ConsumeTask(RespondInternal<T>(values, sendPipe));
        }

        public virtual void Respond<T>(T message)
            where T : class
        {
            AddConsumeTask(RespondInternal(message));
        }

        public virtual async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var sendEndpoint = await ReceiveContext.SendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ConsumeSendEndpoint(sendEndpoint, this);
        }

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return ReceiveContext.NotifyConsumed(context, duration, consumerType);
        }

        public virtual async Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            switch (exception)
            {
                case OperationCanceledException canceled when canceled.CancellationToken == context.CancellationToken:
                    break;

                default:
                    if (!context.CancellationToken.IsCancellationRequested)
                        await GenerateFault(context, exception).ConfigureAwait(false);
                    break;
            }

            await ReceiveContext.NotifyFaulted(context, duration, consumerType, exception).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return ReceiveContext.SendEndpointProvider.ConnectSendObserver(observer);
        }

        public abstract void AddConsumeTask(Task task);

        Task RespondInternal<T>(T message, IPipe<SendContext<T>>? pipe = null)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = this.GetResponseEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe.IsNotEmpty()
                    ? sendEndpoint.Send(message, pipe!, CancellationToken)
                    : sendEndpoint.Send(message, CancellationToken);
            }

            async Task RespondInternalAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe.IsNotEmpty())
                    await sendEndpoint.Send(message, pipe!, CancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send(message, CancellationToken).ConfigureAwait(false);
            }

            return RespondInternalAsync();
        }

        Task RespondInternal<T>(object values, IPipe<SendContext<T>>? pipe = null)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = this.GetResponseEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe.IsNotEmpty()
                    ? sendEndpoint.Send(values, pipe!, CancellationToken)
                    : sendEndpoint.Send<T>(values, CancellationToken);
            }

            async Task RespondInternalAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe.IsNotEmpty())
                    await sendEndpoint.Send(values, pipe!, CancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send<T>(values, CancellationToken).ConfigureAwait(false);
            }

            return RespondInternalAsync();
        }

        protected virtual Task GenerateFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            return context.GenerateFault(exception);
        }

        Task ConsumeTask(Task task)
        {
            AddConsumeTask(task);

            return task;
        }

        protected override async Task<ISendEndpoint> GetPublishSendEndpoint<T>()
        {
            var publishSendEndpoint = await base.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            return new ConsumeSendEndpoint(publishSendEndpoint, this);
        }
    }
}
