namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Components;


    public class RequestStateMessagePipe :
        IPipe<SendContext>
    {
        readonly BehaviorContext<RequestState> _context;
        readonly object _message;
        readonly string[] _messageType;

        public RequestStateMessagePipe(BehaviorContext<RequestState> context, object message, string[] messageType)
        {
            _context = context;

            _message = message;
            _messageType = messageType;
        }

        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(SendContext context)
        {
            context.DestinationAddress = _context.Saga.ResponseAddress;
            context.SourceAddress = _context.Saga.SagaAddress;
            context.FaultAddress = _context.Saga.FaultAddress;
            context.RequestId = _context.Saga.CorrelationId;

            if (_context.Saga.ConversationId.HasValue)
                context.ConversationId = _context.Saga.ConversationId;

            if (_context.Saga.ExpirationTime.HasValue)
            {
                var timeToLive = DateTime.UtcNow - _context.Saga.ExpirationTime.Value;
                context.TimeToLive = timeToLive > TimeSpan.Zero ? timeToLive : TimeSpan.FromSeconds(1);
            }

            context.Serializer = _context.SerializerContext.GetMessageSerializer(_message, _messageType);
        }
    }
}
