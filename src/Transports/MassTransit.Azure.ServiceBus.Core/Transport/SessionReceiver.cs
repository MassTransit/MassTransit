namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using global::Azure.Messaging.ServiceBus;


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
            LogContext.Debug?.Log("Receiving {SessionId}:{MessageId}({EntityPath})", message.SessionId, message.MessageId, _context.EntityPath);

            await _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageSession, message))
                .ConfigureAwait(false);
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
