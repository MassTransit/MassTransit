namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Util;
    using Transports;


    public class TestConsumeContext<TMessage> :
        BasePipeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        ReceiveContext _receiveContext;

        public TestConsumeContext(TMessage message)
        {
            Message = message;

            MessageId = NewId.NextGuid();
            SourceAddress = new Uri("loopback://localhost/input_queue");
            DestinationAddress = new Uri("loopback://localhost/input_queue");

            _receiveContext = new TestReceiveContext(SourceAddress);
        }

        public Guid? MessageId { get; }

        public Guid? RequestId { get; }

        public Guid? CorrelationId { get; }

        public Guid? ConversationId { get; }

        public Guid? InitiatorId { get; }

        public DateTime? ExpirationTime { get; }

        public Uri SourceAddress { get; }

        public Uri DestinationAddress { get; }

        public Uri ResponseAddress { get; }

        public Uri FaultAddress { get; }

        public DateTime? SentTime { get; }

        public Headers Headers { get; }

        public HostInfo Host { get; } = null;

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public ReceiveContext ReceiveContext
        {
            get { return _receiveContext; }
        }

        public Task ConsumeCompleted
        {
            get { return Task.FromResult(true); }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return Enumerable.Repeat(MessageUrn.ForTypeString<TMessage>(), 1); }
        }

        public bool HasMessageType(Type messageType)
        {
            return messageType.GetTypeInfo().IsAssignableFrom(typeof(TMessage));
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            consumeContext = this as ConsumeContext<T>;
            return consumeContext != null;
        }

        public void AddConsumeTask(Task task)
        {

        }

        public Task RespondAsync<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public void Respond<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
        }

        public async Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
        }

        public TMessage Message { get; }

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return new Connectable<IPublishObserver>().Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new Connectable<ISendObserver>().Connect(observer);
        }
    }


    public class TestReceiveContext :
        BaseReceiveContext
    {
        public TestReceiveContext(Uri sourceAddress)
            : base(sourceAddress, false, null)
        {
            HeaderProvider = new DictionaryHeaderProvider(new Dictionary<string, object>());
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public override byte[] GetBody()
        {
            return new byte[0];
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream();
        }
    }
}
