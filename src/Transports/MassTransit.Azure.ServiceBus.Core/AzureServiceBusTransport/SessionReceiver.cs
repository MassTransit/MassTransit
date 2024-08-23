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

            MessageLockContext lockContext = new ServiceBusSessionMessageLockContext(messageSession, message);
            MessageSessionContext sessionContext = new ServiceBusMessageSessionContext(messageSession);
            var context = new ServiceBusReceiveContext(message, _context, lockContext, _clientContext, sessionContext);

            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

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
                registration.Dispose();
                context.Dispose();
            }
        }
    }
}
