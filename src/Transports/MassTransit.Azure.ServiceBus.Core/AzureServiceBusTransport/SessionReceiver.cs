namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    public class SessionReceiver :
        Receiver
    {
        readonly ClientContext _clientContext;
        readonly ServiceBusReceiveEndpointContext _context;

        public SessionReceiver(ClientContext clientContext, ServiceBusReceiveEndpointContext context)
            : base(clientContext, context)
        {
            _clientContext = clientContext;
            _context = context;
        }

        public override void Start()
        {
            _clientContext.OnSessionAsync(OnSession, ExceptionHandler);

            SetReady(_clientContext.StartAsync());
        }

        async Task OnSession(ProcessSessionMessageEventArgs messageSession, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            if (IsStopping)
                return;

            MessageLockContext lockContext = new ServiceBusSessionMessageLockContext(messageSession, message, Stopped);
            MessageSessionContext sessionContext = new ServiceBusMessageSessionContext(messageSession, Stopped);
            var context = new ServiceBusReceiveContext(message, _context, lockContext, _clientContext, sessionContext);

            CancellationTokenSource cancellationTokenSource = null;
            CancellationTokenRegistration timeoutRegistration = default;
            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
            {
                void Callback()
                {
                    if (_context.ConsumerStopTimeout.HasValue)
                    {
                        cancellationTokenSource = new CancellationTokenSource(_context.ConsumerStopTimeout.Value);
                        timeoutRegistration = cancellationTokenSource.Token.Register(context.Cancel);
                    }
                    else
                        context.Cancel();
                }

                registration = cancellationToken.Register(Callback);
            }

            try
            {
                await Dispatch(message, context, lockContext).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // do NOT let exceptions propagate to the Azure SDK
            }
            finally
            {
                timeoutRegistration.Dispose();
                registration.Dispose();

                cancellationTokenSource?.Dispose();

                context.Dispose();
            }
        }
    }
}
