namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    public class SessionReceiver :
        Receiver
    {
        readonly ClientContext _context;
        readonly IServiceBusMessageReceiver _messageReceiver;

        public SessionReceiver(ClientContext context, IServiceBusMessageReceiver messageReceiver)
            : base(context, messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;
        }

        public override Task Start()
        {
            _context.OnSessionAsync(OnSession, ExceptionHandler);

            SetReady();

            return _context.StartAsync();
        }

        async Task OnSession(ProcessSessionMessageEventArgs messageSession, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageSession, message))
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                // do NOT let exceptions propagate to the Azure SDK
            }
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, ProcessSessionMessageEventArgs messageSession, ServiceBusReceivedMessage message)
        {
            MessageSessionContext sessionContext = new ServiceBusMessageSessionContext(messageSession);
            MessageLockContext lockContext = new ServiceBusSessionMessageLockContext(messageSession, message);

            receiveContext.GetOrAddPayload(() => sessionContext);
            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context);
        }
    }
}
