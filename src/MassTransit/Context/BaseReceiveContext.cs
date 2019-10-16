namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Serialization;
    using Topology;
    using Transports;
    using Util;


    public abstract class BaseReceiveContext :
        ScopePipeContext,
        ReceiveContext,
        IDisposable
    {
        static readonly ContentType DefaultContentType = JsonMessageSerializer.JsonContentType;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Lazy<ContentType> _contentType;
        readonly Lazy<Headers> _headers;
        readonly PendingTaskCollection _receiveTasks;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly Stopwatch _receiveTimer;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        protected BaseReceiveContext(Uri inputAddress, bool redelivered, ReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(receiveEndpointContext, payloads)
        {
            _receiveTimer = Stopwatch.StartNew();

            _cancellationTokenSource = new CancellationTokenSource();
            _receiveEndpointContext = receiveEndpointContext;

            InputAddress = inputAddress;
            Redelivered = redelivered;

            _headers = new Lazy<Headers>(() => new JsonHeaders(ObjectTypeDeserializer.Instance, HeaderProvider));

            _contentType = new Lazy<ContentType>(GetContentType);
            _receiveTasks = new PendingTaskCollection(4);

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(GetSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(GetPublishEndpointProvider);
        }

        protected abstract IHeaderProvider HeaderProvider { get; }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        CancellationToken PipeContext.CancellationToken => _cancellationTokenSource.Token;

        public bool IsDelivered { get; private set; }
        public bool IsFaulted { get; private set; }
        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;
        public IPublishTopology PublishTopology => _receiveEndpointContext.Publish;

        public Task ReceiveCompleted => _receiveTasks.Completed(CancellationToken);

        public void AddReceiveTask(Task task)
        {
            _receiveTasks.Add(task);
        }

        public bool Redelivered { get; }
        public Headers TransportHeaders => _headers.Value;

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            IsDelivered = true;

            context.LogConsumed(duration, consumerType);

            return _receiveEndpointContext.ReceiveObservers.PostConsume(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            IsFaulted = true;

            context.LogFaulted(duration, consumerType, exception);

            GetOrAddPayload<ConsumerFaultInfo>(() => new FaultInfo(TypeMetadataCache<T>.ShortName, consumerType));

            return _receiveEndpointContext.ReceiveObservers.ConsumeFault(context, duration, consumerType, exception);
        }

        public virtual Task NotifyFaulted(Exception exception)
        {
            IsFaulted = true;

            this.LogFaulted(exception);

            return _receiveEndpointContext.ReceiveObservers.ReceiveFault(this, exception);
        }

        public abstract byte[] GetBody();
        public abstract Stream GetBodyStream();

        public TimeSpan ElapsedTime => _receiveTimer.Elapsed;
        public Uri InputAddress { get; }
        public ContentType ContentType => _contentType.Value;

        protected virtual ISendEndpointProvider GetSendEndpointProvider()
        {
            return _receiveEndpointContext.SendEndpointProvider;
        }

        protected virtual IPublishEndpointProvider GetPublishEndpointProvider()
        {
            return _receiveEndpointContext.PublishEndpointProvider;
        }

        protected virtual ContentType GetContentType()
        {
            if (_headers.Value.TryGetHeader("Content-Type", out var contentTypeHeader))
            {
                if (contentTypeHeader is ContentType contentType)
                    return contentType;

                if (contentTypeHeader is string contentTypeString)
                    return new ContentType(contentTypeString);
            }

            return DefaultContentType;
        }

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
