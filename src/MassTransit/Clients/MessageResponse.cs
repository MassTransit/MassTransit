namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// A result from a request
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class MessageResponse<TResult> :
        Response<TResult>
        where TResult : class
    {
        readonly ConsumeContext<TResult> _context;

        public MessageResponse(ConsumeContext<TResult> context)
        {
            _context = context;

            Message = context.Message;
        }

        Guid? MessageContext.MessageId => _context.MessageId;
        Guid? MessageContext.RequestId => _context.RequestId;
        Guid? MessageContext.CorrelationId => _context.CorrelationId;
        Guid? MessageContext.ConversationId => _context.ConversationId;
        Guid? MessageContext.InitiatorId => _context.InitiatorId;
        DateTime? MessageContext.ExpirationTime => _context.ExpirationTime;
        Uri MessageContext.SourceAddress => _context.SourceAddress;
        Uri MessageContext.DestinationAddress => _context.DestinationAddress;
        Uri MessageContext.ResponseAddress => _context.ResponseAddress;
        Uri MessageContext.FaultAddress => _context.FaultAddress;
        DateTime? MessageContext.SentTime => _context.SentTime;
        Headers MessageContext.Headers => _context.Headers;
        HostInfo MessageContext.Host => _context.Host;

        public TResult Message { get; }
        object Response.Message => Message;

        public T DeserializeObject<T>(Dictionary<string, object> dictionary)
            where T : class
        {
            return _context.SerializerContext.DeserializeObject<T>(dictionary);
        }
    }
}
