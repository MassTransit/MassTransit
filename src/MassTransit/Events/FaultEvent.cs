namespace MassTransit.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    [Serializable]
    public class FaultEvent<T> :
        Fault<T>
    {
        protected FaultEvent()
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

        public Guid FaultId { get; private set; }
        public Guid? FaultedMessageId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public ExceptionInfo[] Exceptions { get; private set; }
        public HostInfo Host { get; private set; }
        public string[] FaultMessageTypes { get; private set; }
        public T Message { get; private set; }

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


    [Serializable]
    public class FaultEvent :
        Fault
    {
        public Guid FaultId { get; private set; }
        public Guid? FaultedMessageId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public ExceptionInfo[] Exceptions { get; private set; }
        public HostInfo Host { get; private set; }
        public string[] FaultMessageTypes { get; private set; }
    }
}
