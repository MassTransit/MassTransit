namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Microsoft.Azure.ServiceBus;


    public class SessionReceiver :
        Receiver
    {
        readonly ClientContext _context;
        readonly IBrokeredMessageReceiver _messageReceiver;

        public SessionReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
            : base(context, messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;
        }

        public override Task Start()
        {
            _context.OnSessionAsync(OnSession, ExceptionHandler);

            SetReady();

            return Task.CompletedTask;
        }

        async Task OnSession(IMessageSession messageSession, Message message, CancellationToken cancellationToken)
        {
            if (IsStopping)
            {
                await WaitForDeliveryComplete().ConfigureAwait(false);
                return;
            }

            LogContext.Debug?.Log("Receiving {SessionId}:{MessageId}({EntityPath})", message.SessionId, message.MessageId, _context.EntityPath);

            await _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageSession, message))
                .ConfigureAwait(false);
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, IMessageSession messageSession, Message message)
        {
            MessageSessionContext sessionContext = new BrokeredMessageSessionContext(messageSession);
            MessageLockContext lockContext = new SessionMessageLockContext(messageSession, message);

            receiveContext.GetOrAddPayload(() => sessionContext);
            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context);
        }
    }
}
