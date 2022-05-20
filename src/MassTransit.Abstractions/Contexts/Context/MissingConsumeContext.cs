namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class MissingConsumeContext :
        ConsumeContext
    {
        public static ConsumeContext Instance { get; } = new MissingConsumeContext();

        public bool HasPayloadType(Type payloadType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public bool TryGetPayload<T>(out T? payload)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public CancellationToken CancellationToken => throw new ConsumeContextNotAvailableException();

        public Guid? MessageId => throw new ConsumeContextNotAvailableException();

        public Guid? RequestId => throw new ConsumeContextNotAvailableException();

        public Guid? CorrelationId => throw new ConsumeContextNotAvailableException();

        public Guid? ConversationId => throw new ConsumeContextNotAvailableException();

        public Guid? InitiatorId => throw new ConsumeContextNotAvailableException();

        public DateTime? ExpirationTime => throw new ConsumeContextNotAvailableException();

        public Uri SourceAddress => throw new ConsumeContextNotAvailableException();

        public Uri DestinationAddress => throw new ConsumeContextNotAvailableException();

        public Uri ResponseAddress => throw new ConsumeContextNotAvailableException();

        public Uri FaultAddress => throw new ConsumeContextNotAvailableException();

        public DateTime? SentTime => throw new ConsumeContextNotAvailableException();

        public Headers Headers => throw new ConsumeContextNotAvailableException();

        public HostInfo Host => throw new ConsumeContextNotAvailableException();

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public ReceiveContext ReceiveContext => throw new ConsumeContextNotAvailableException();
        public SerializerContext SerializerContext => throw new ConsumeContextNotAvailableException();

        public Task ConsumeCompleted => throw new ConsumeContextNotAvailableException();

        public IEnumerable<string> SupportedMessageTypes => throw new ConsumeContextNotAvailableException();

        public bool HasMessageType(Type messageType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public void AddConsumeTask(Task task)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(T message)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync(object message)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync(object message, Type messageType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(object values)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public void Respond<T>(T message)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            throw new ConsumeContextNotAvailableException();
        }
    }
}
