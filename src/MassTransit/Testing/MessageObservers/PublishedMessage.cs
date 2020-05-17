namespace MassTransit.Testing.MessageObservers
{
    using System;


    public class PublishedMessage<T> :
        IPublishedMessage<T>
        where T : class
    {
        readonly PublishContext<T> _context;

        public PublishedMessage(PublishContext<T> context, Exception exception = null)
        {
            _context = context;
            Exception = exception;
        }

        Guid? IAsyncListElement.ElementId => _context.MessageId;

        SendContext IPublishedMessage.Context => _context;
        public Exception Exception { get; }
        public Type MessageType => typeof(T);
        object IPublishedMessage.MessageObject => _context.Message;

        PublishContext<T> IPublishedMessage<T>.Context => _context;
    }
}
