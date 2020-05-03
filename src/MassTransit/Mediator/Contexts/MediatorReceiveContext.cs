namespace MassTransit.Mediator.Contexts
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Serialization;
    using Topology;
    using Transports;
    using Util;


    static class MediatorReceiveContext
    {
        const string ContentTypeHeaderValue = "application/vnd.masstransit+obj";
        internal static readonly ContentType ObjectContentType = new ContentType(ContentTypeHeaderValue);
    }


    public sealed class MediatorReceiveContext<TMessage> :
        BasePipeContext,
        ReceiveContext
        where TMessage : class
    {
        readonly IReceiveObserver _observers;
        readonly PendingTaskCollection _receiveTasks;
        readonly Stopwatch _receiveTimer;
        readonly MessageIdMessageHeader _headers;
        readonly MediatorConsumeContext<TMessage> _consumeContext;

        public MediatorReceiveContext(SendContext<TMessage> sendContext, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider, IPublishTopology publishTopology, IReceiveObserver observers, CancellationToken cancellationToken,
            params object[] payloads)
            : base(cancellationToken, payloads)
        {
            _observers = observers;

            SendEndpointProvider = sendEndpointProvider;
            PublishEndpointProvider = publishEndpointProvider;
            PublishTopology = publishTopology;

            _receiveTimer = Stopwatch.StartNew();

            var messageId = sendContext.MessageId ?? throw new ArgumentNullException(nameof(MessageContext.MessageId));

            _headers = new MessageIdMessageHeader(messageId);

            _receiveTasks = new PendingTaskCollection(4);

            _consumeContext = new MediatorConsumeContext<TMessage>(this, sendContext);

            AddOrUpdatePayload<ConsumeContext>(() => _consumeContext, existing => _consumeContext);
        }

        public bool IsDelivered { get; private set; }
        public bool IsFaulted { get; private set; }

        public Stream GetBodyStream()
        {
            throw new NotImplementedByDesignException("The mediator should not be serializing messages");
        }

        public byte[] GetBody()
        {
            throw new NotImplementedByDesignException("The mediator should not be serializing messages");
        }

        public Task ReceiveCompleted => _receiveTasks.Completed(CancellationToken);

        public void AddReceiveTask(Task task)
        {
            _receiveTasks.Add(task);
        }

        public ISendEndpointProvider SendEndpointProvider { get; }
        public IPublishEndpointProvider PublishEndpointProvider { get; }
        public IPublishTopology PublishTopology { get; }

        public bool Redelivered => false;
        public Headers TransportHeaders => _headers;

        public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            IsDelivered = true;

            context.LogConsumed(duration, consumerType);

            return _observers.PostConsume(context, duration, consumerType);
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            IsFaulted = true;

            context.LogFaulted(duration, consumerType, exception);

            GetOrAddPayload<ConsumerFaultInfo>(() => new FaultInfo(TypeMetadataCache<T>.ShortName, consumerType));

            return _observers.ConsumeFault(context, duration, consumerType, exception);
        }

        public Task NotifyFaulted(Exception exception)
        {
            IsFaulted = true;

            this.LogFaulted(exception);

            return _observers.ReceiveFault(this, exception);
        }

        public TimeSpan ElapsedTime => _receiveTimer.Elapsed;
        public Uri InputAddress => _consumeContext.DestinationAddress;
        public ContentType ContentType => MediatorReceiveContext.ObjectContentType;


        class FaultInfo :
            ConsumerFaultInfo
        {
            public FaultInfo(string messageType, string consumerType)
            {
                MessageType = messageType;
                ConsumerType = consumerType;
            }

            public string MessageType { get; }
            public string ConsumerType { get; }
        }
    }
}
