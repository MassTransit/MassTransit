namespace MassTransit.Testing
{
    using System;


    public class SentMessage<T> :
        ISentMessage<T>
        where T : class
    {
        readonly SendContext<T> _context;
        readonly Exception _exception;

        public SentMessage(SendContext<T> context, Exception exception = null)
        {
            _context = context;
            _exception = exception;

            StartTime = context.SentTime ?? DateTime.UtcNow;
            ElapsedTime = DateTime.UtcNow - StartTime;
        }

        Guid? IAsyncListElement.ElementId => _context.MessageId;
        SendContext ISentMessage.Context => _context;
        public DateTime StartTime { get; }
        public TimeSpan ElapsedTime { get; }
        object ISentMessage.MessageObject => _context.Message;
        Exception ISentMessage.Exception => _exception;
        Type ISentMessage.MessageType => typeof(T);
        public string ShortTypeName => TypeCache<T>.ShortName;
        SendContext<T> ISentMessage<T>.Context => _context;
    }
}
