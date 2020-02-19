namespace MassTransit.Transports
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
    using Util;


    public sealed class DeliveryReceiveContext<TMessage> :
        BasePipeContext,
        ReceiveContext,
        IDisposable
        where TMessage : class
    {
        const string ContentTypeHeaderValue = "application/vnd.masstransit+obj";
        static readonly ContentType ObjectContentType = new ContentType(ContentTypeHeaderValue);

        readonly IReceiveObserver _observers;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly PendingTaskCollection _receiveTasks;
        readonly Stopwatch _receiveTimer;
        CancellationTokenRegistration _registration;
        readonly MessageIdMessageHeader _headers;
        readonly DeliveryConsumeContext<TMessage> _consumeContext;

        public DeliveryReceiveContext(SendContext<TMessage> sendContext, IReceiveObserver observers, CancellationToken cancellationToken,
            params object[] payloads)
            : base(payloads)
        {
            _observers = observers;
            _receiveTimer = Stopwatch.StartNew();

            var messageId = NewId.NextGuid();

            _headers = new MessageIdMessageHeader(messageId);

            _cancellationTokenSource = new CancellationTokenSource();
            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);

            _receiveTasks = new PendingTaskCollection(4);

            _consumeContext = new DeliveryConsumeContext<TMessage>(this, sendContext);

            AddOrUpdatePayload<ConsumeContext>(() => _consumeContext, existing => _consumeContext);
        }

        public void Dispose()
        {
            _registration.Dispose();
            _cancellationTokenSource.Dispose();
        }

        CancellationToken PipeContext.CancellationToken => _cancellationTokenSource.Token;

        public bool IsDelivered { get; private set; }
        public bool IsFaulted { get; private set; }

        public Stream GetBodyStream()
        {
            return new MemoryStream();
        }

        public byte[] GetBody()
        {
            return new byte[0];
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
        public ContentType ContentType => ObjectContentType;

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }


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
