namespace MassTransit.Testing
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

            ElapsedTime = context.ReceiveContext.ElapsedTime;
            StartTime = DateTime.UtcNow - ElapsedTime;
            if (StartTime < context.SentTime)
                StartTime = context.SentTime.Value;
        }

        Guid? IAsyncListElement.ElementId => _context.MessageId;
        ConsumeContext IReceivedMessage.Context => _context;
        public DateTime StartTime { get; }
        public TimeSpan ElapsedTime { get; }
        Exception IReceivedMessage.Exception => _exception;
        Type IReceivedMessage.MessageType => typeof(T);
        string IReceivedMessage.ShortTypeName => TypeCache<T>.ShortName;
        object IReceivedMessage.MessageObject => _context.Message;
        ConsumeContext<T> IReceivedMessage<T>.Context => _context;
    }
}
