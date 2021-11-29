namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Transports;


    public class ServiceBusMessageReceiver :
        IServiceBusMessageReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public ServiceBusMessageReceiver(ReceiveEndpointContext context)
        {
            _context = context;

            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");

            _context.ReceivePipe.Probe(scope);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public async Task Handle(ServiceBusReceivedMessage message, CancellationToken cancellationToken,
            Action<ReceiveContext> contextCallback)
        {
            var context = new ServiceBusReceiveContext(message, _context);
            contextCallback?.Invoke(context);

            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            var receiveLock = context.TryGetPayload<MessageLockContext>(out var lockContext)
                ? new ServiceBusReceiveLockContext(lockContext, context)
                : default;

            try
            {
                await _dispatcher.Dispatch(context, receiveLock).ConfigureAwait(false);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.SessionLockLost)
            {
                LogContext.Error?.Log("Session Lock Lost: {InputAddress} {MessageId} {SequenceNumber} ({SessionId})", _context.InputAddress,
                    message.MessageId, message.SequenceNumber, message.SessionId);

                await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                throw;
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessageLockLost)
            {
                LogContext.Error?.Log("Message Lock Lost: {InputAddress} {MessageId} {SequenceNumber}", _context.InputAddress, message.MessageId,
                    message.SequenceNumber);

                await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                throw;
            }
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
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
