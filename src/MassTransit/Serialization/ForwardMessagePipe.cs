namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Transports;


    public class ForwardMessagePipe<TMessage> :
        IPipe<SendContext<TMessage>>,
        ISendPipe
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly IPipe<SendContext<TMessage>> _pipe;

        public ForwardMessagePipe(ConsumeContext<TMessage> context, IPipe<SendContext<TMessage>> pipe = default)
        {
            _context = context;
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext<TMessage> context)
        {
            context.MessageId = _context.MessageId;
            context.RequestId = _context.RequestId;
            context.ConversationId = _context.ConversationId;
            context.CorrelationId = _context.CorrelationId;
            context.InitiatorId = _context.InitiatorId;
            context.SourceAddress = _context.SourceAddress;
            context.ResponseAddress = _context.ResponseAddress;
            context.FaultAddress = _context.FaultAddress;

            if (_context.ExpirationTime.HasValue)
                context.TimeToLive = _context.ExpirationTime.Value.ToUniversalTime() - DateTime.UtcNow;

            foreach (KeyValuePair<string, object> header in _context.Headers.GetAll())
                context.Headers.Set(header.Key, header.Value);

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);

            var forwarderAddress = _context.ReceiveContext.InputAddress ?? _context.DestinationAddress;
            if (forwarderAddress != null && forwarderAddress != context.DestinationAddress)
                context.Headers.Set(MessageHeaders.ForwarderAddress, forwarderAddress.ToString());

            if (_context.SerializerContext != null)
                context.Serializer = _context.SerializerContext.GetMessageSerializer();
            else
                context.Serializer = new CopyBodySerializer(_context.ReceiveContext.ContentType, _context.ReceiveContext.Body);
        }

        public Task Send<T>(SendContext<T> context)
            where T : class
        {
            return _pipe is ISendContextPipe sendContextPipe
                ? sendContextPipe.Send(context)
                : Task.CompletedTask;
        }
    }
}
