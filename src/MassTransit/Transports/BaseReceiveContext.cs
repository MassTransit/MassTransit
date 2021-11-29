namespace MassTransit.Transports
{
    using System;
    using System.Diagnostics;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;
    using Util;


    public abstract class BaseReceiveContext :
        ScopePipeContext,
        ReceiveContext,
        IDisposable
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Lazy<ContentType> _contentType;
        readonly Lazy<Headers> _headers;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly ReceiveEndpointContext _receiveEndpointContext;
        readonly PendingTaskCollection _receiveTasks;
        readonly Stopwatch _receiveTimer;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;

        protected BaseReceiveContext(bool redelivered, ReceiveEndpointContext receiveEndpointContext, params object[] payloads)
            : base(receiveEndpointContext, payloads)
        {
            _receiveTimer = Stopwatch.StartNew();

            _cancellationTokenSource = new CancellationTokenSource();
            _receiveEndpointContext = receiveEndpointContext;

            InputAddress = receiveEndpointContext.InputAddress;
            Redelivered = redelivered;

            _headers = new Lazy<Headers>(() => new JsonTransportHeaders(HeaderProvider));

            _contentType = new Lazy<ContentType>(GetContentType);
            _receiveTasks = new PendingTaskCollection(4);

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(GetSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(GetPublishEndpointProvider);
        }

        protected abstract IHeaderProvider HeaderProvider { get; }

        public virtual void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        public override CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public bool IsDelivered { get; private set; }
        public bool IsFaulted { get; private set; }

        public bool PublishFaults => _receiveEndpointContext.PublishFaults;
        public abstract MessageBody Body { get; }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

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

            GetOrAddPayload<ConsumerFaultContext>(() => new FaultContext(TypeCache<T>.ShortName, consumerType));

            return _receiveEndpointContext.ReceiveObservers.ConsumeFault(context, duration, consumerType, exception);
        }

        public virtual Task NotifyFaulted(Exception exception)
        {
            IsFaulted = true;

            this.LogFaulted(exception);

            return _receiveEndpointContext.ReceiveObservers.ReceiveFault(this, exception);
        }

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
            if (_headers.Value.TryGetHeader("Content-Type", out var contentTypeHeader) || _headers.Value.TryGetHeader("ContentType", out contentTypeHeader))
            {
                if (contentTypeHeader is ContentType contentType)
                    return contentType;

                if (contentTypeHeader is string contentTypeString)
                    return ConvertToContentType(contentTypeString);
            }

            return default;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        protected static ContentType ConvertToContentType(string text)
        {
            try
            {
                return new ContentType(text);
            }
            catch (FormatException)
            {
                return default;
            }
        }


        class FaultContext :
            ConsumerFaultContext
        {
            public FaultContext(string messageType, string consumerType)
            {
                MessageType = messageType;
                ConsumerType = consumerType;
            }

            public string MessageType { get; }
            public string ConsumerType { get; }
        }
    }
}
