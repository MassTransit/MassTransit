namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class CopyContextPipe :
        IPipe<SendContext>
    {
        readonly Action<ConsumeContext, SendContext>? _callback;
        readonly ConsumeContext _context;

        public CopyContextPipe(ConsumeContext context, Action<ConsumeContext, SendContext>? callback = null)
        {
            _context = context;
            _callback = callback;
        }

        public Task Send(SendContext context)
        {
            context.MessageId = _context.MessageId;
            context.RequestId = _context.RequestId;
            context.CorrelationId = _context.CorrelationId;
            context.ConversationId = _context.ConversationId;
            context.InitiatorId = _context.InitiatorId;
            context.SourceAddress = _context.SourceAddress;
            context.ResponseAddress = _context.ResponseAddress;
            context.FaultAddress = _context.FaultAddress;

            if (_context.ExpirationTime.HasValue)
                context.TimeToLive = _context.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

            foreach (KeyValuePair<string, object> header in _context.Headers.GetAll())
                context.Headers.Set(header.Key, header.Value);

            _callback?.Invoke(_context, context);

            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("copyContext");
        }
    }
}
