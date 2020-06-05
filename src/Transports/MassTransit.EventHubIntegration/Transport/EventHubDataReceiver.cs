namespace MassTransit.EventHubIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Transports;
    using Transports.Metrics;


    public class EventHubDataReceiver :
        IEventHubDataReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public EventHubDataReceiver(ReceiveEndpointContext context)
        {
            _context = context;
            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "event-data");
        }

        public async Task Handle(ProcessEventArgs @event, CancellationToken cancellationToken)
        {
            if (!@event.HasEvent)
                return;

            var context = new EventDataReceiveContext(@event.Data, _context);

            CancellationTokenRegistration registration;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            var receiveLock = context.TryGetPayload(out IProcessorLockContext lockContext)
                ? new ProcessEventLockContext(lockContext, @event, cancellationToken)
                : default;

            try
            {
                await _dispatcher.Dispatch(context, receiveLock).ConfigureAwait(false);
            }
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
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
