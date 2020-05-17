namespace MassTransit.Testing.MessageObservers
{
    using System;


    public class ReceivedMessage<T> :
        IReceivedMessage<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly Exception _exception;

        public ReceivedMessage(ConsumeContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;
        }

        Guid? IAsyncListElement.ElementId => _context.MessageId;

        ConsumeContext IReceivedMessage.Context => _context;
        Exception IReceivedMessage.Exception => _exception;
        Type IReceivedMessage.MessageType => typeof(T);
        object IReceivedMessage.MessageObject => _context.Message;

        ConsumeContext<T> IReceivedMessage<T>.Context => _context;
    }
}
