namespace MassTransit.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [Serializable]
    public class FaultEvent<T> :
        Fault<T>
    {
        public FaultEvent(T message, Guid? faultedMessageId, HostInfo host, Exception exception, string[] faultMessageTypes)
            : this(message, faultedMessageId, host, GetExceptions(exception), faultMessageTypes)
        {
        }

        public FaultEvent(T message, Guid? faultedMessageId, HostInfo host, IEnumerable<ExceptionInfo> exceptions, string[] faultMessageTypes)
        {
            Timestamp = DateTime.UtcNow;
            FaultId = NewId.NextGuid();

            Message = message;
            Host = host;
            FaultMessageTypes = faultMessageTypes;
            FaultedMessageId = faultedMessageId;

            Exceptions = exceptions.ToArray();
        }

        public Guid FaultId { get; }
        public Guid? FaultedMessageId { get; }
        public DateTime Timestamp { get; }
        public ExceptionInfo[] Exceptions { get; }
        public HostInfo Host { get; }
        public string[] FaultMessageTypes { get; }
        public T Message { get; }

        static ExceptionInfo[] GetExceptions(Exception exception)
        {
            var aggregateException = exception as AggregateException;

            return aggregateException?.InnerExceptions
                    .Where(x => x != null)
                    .Select(x => (ExceptionInfo)new FaultExceptionInfo(x))
                    .ToArray()
                ?? new ExceptionInfo[] {new FaultExceptionInfo(exception)};
        }
    }
}
