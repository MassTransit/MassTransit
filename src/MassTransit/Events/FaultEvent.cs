namespace MassTransit.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [Serializable]
    public class FaultEvent<T> :
        Fault<T>
    {
        public FaultEvent()
        {
        }

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

        public Guid FaultId { get; set; }
        public Guid? FaultedMessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public ExceptionInfo[] Exceptions { get; set; }
        public HostInfo Host { get; set; }
        public string[] FaultMessageTypes { get; set; }
        public T Message { get; set; }

        static ExceptionInfo[] GetExceptions(Exception exception)
        {
            var aggregateException = exception as AggregateException;

            return aggregateException?.InnerExceptions
                    .Where(x => x != null)
                    .Select(x => (ExceptionInfo)new FaultExceptionInfo(x))
                    .ToArray()
                ?? new ExceptionInfo[] { new FaultExceptionInfo(exception) };
        }
    }


    [Serializable]
    public class FaultEvent :
        Fault
    {
        public Guid FaultId { get; set; }
        public Guid? FaultedMessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public ExceptionInfo[] Exceptions { get; set; }
        public HostInfo Host { get; set; }
        public string[] FaultMessageTypes { get; set; }
    }
}
