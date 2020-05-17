namespace MassTransit.Testing.MessageObservers
{
    using System;


    public class SentMessage<TMessage> :
        ISentMessage<TMessage>
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        readonly Exception _exception;

        public SentMessage(SendContext<TMessage> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        Guid? IAsyncListElement.ElementId => _context.MessageId;

        SendContext ISentMessage.Context => _context;
        object ISentMessage.MessageObject => _context.Message;
        Exception ISentMessage.Exception => _exception;
        Type ISentMessage.MessageType => typeof(TMessage);

        SendContext<TMessage> ISentMessage<TMessage>.Context => _context;
    }
}
