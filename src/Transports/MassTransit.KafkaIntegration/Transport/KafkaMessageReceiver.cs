namespace MassTransit.KafkaIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using Metadata;
    using Pipeline;
    using Serializers;
    using Transports;
    using Transports.Metrics;


    public class KafkaMessageReceiver<TKey, TValue> :
        IKafkaMessageReceiver<TKey, TValue>
        where TValue : class
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;

        public KafkaMessageReceiver(ReceiveEndpointContext context, IConsumerLockContext<TKey, TValue> lockContext, IHeadersDeserializer headersDeserializer)
        {
            _context = context;
            _lockContext = lockContext;
            _headersDeserializer = headersDeserializer;
            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("keyType", TypeMetadataCache<TKey>.ShortName);
            scope.Add("valueType", TypeMetadataCache<TValue>.ShortName);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public async Task Handle(ConsumeResult<TKey, TValue> result, CancellationToken cancellationToken)
        {
            var context = new ConsumeResultReceiveContext<TKey, TValue>(result, _context, _lockContext, _headersDeserializer);

            CancellationTokenRegistration registration;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _context.ConnectReceiveTransportObserver(observer);
        }

        public int ActiveDispatchCount => _dispatcher.ActiveDispatchCount;
        public long DispatchCount => _dispatcher.DispatchCount;
        public int MaxConcurrentDispatchCount => _dispatcher.MaxConcurrentDispatchCount;

        public event ZeroActiveDispatchHandler ZeroActivity
        {
            add => _dispatcher.ZeroActivity += value;
            remove => _dispatcher.ZeroActivity -= value;
        }

        public DeliveryMetrics GetMetrics()
        {
            return _dispatcher.GetMetrics();
        }
    }
}
