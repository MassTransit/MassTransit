namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using InMemoryTransport;
    using InMemoryTransport.Configuration;
    using Logging;
    using Middleware;
    using Transports;
    using Util;


    public static class TestConsumeContext
    {
        static InMemoryReceiveEndpointContext _receiveEndpointContext;

        static InMemoryReceiveEndpointContext Build()
        {
            var topologyConfiguration = new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology);
            IInMemoryBusConfiguration busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, null);

            var receiveEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("input-queue");

            var hostTopology = new InMemoryBusTopology(busConfiguration.HostConfiguration, topologyConfiguration);
            var host = new InMemoryHost(busConfiguration.HostConfiguration, hostTopology);

            var builder = new InMemoryReceiveEndpointBuilder(busConfiguration.HostConfiguration, receiveEndpointConfiguration);

            if (LogContext.Current == null)
            {
                var loggerFactory = new TestOutputLoggerFactory(true);

                LogContext.ConfigureCurrentLogContext(loggerFactory);
            }

            return builder.CreateReceiveEndpointContext();
        }

        public static InMemoryReceiveEndpointContext GetContext()
        {
            return _receiveEndpointContext ??= Build();
        }
    }


    public class TestConsumeContext<TMessage> :
        BasePipeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        public TestConsumeContext(TMessage message, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Message = message;

            MessageId = NewId.NextGuid();
            SourceAddress = new Uri("loopback://localhost/input_queue");
            DestinationAddress = new Uri("loopback://localhost/input_queue");

            ReceiveContext = new TestReceiveContext(TestConsumeContext.GetContext());
        }

        public TestConsumeContext(TMessage message)
        {
            Message = message;

            MessageId = NewId.NextGuid();
            SourceAddress = new Uri("loopback://localhost/input_queue");
            DestinationAddress = new Uri("loopback://localhost/input_queue");

            ReceiveContext = new TestReceiveContext(TestConsumeContext.GetContext());
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

        public ReceiveContext ReceiveContext { get; }
        public SerializerContext SerializerContext { get; }

        public Task ConsumeCompleted => Task.FromResult(true);

        public IEnumerable<string> SupportedMessageTypes => Enumerable.Repeat(MessageUrn.ForTypeString<TMessage>(), 1);

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

        public Task RespondAsync<T>(T message)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
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

        public void Respond<T>(T message)
            where T : class
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
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
        public TestReceiveContext(ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            HeaderProvider = new DictionaryHeaderProvider(new Dictionary<string, object>());

            Body = new BytesMessageBody(Array.Empty<byte>());
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public override MessageBody Body { get; }
    }
}
