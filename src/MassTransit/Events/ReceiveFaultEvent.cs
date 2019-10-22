namespace MassTransit.Events
{
    using System;
    using System.Linq;

    [Serializable]
    public class ReceiveFaultEvent :
        ReceiveFault
    {
        public ReceiveFaultEvent(HostInfo host, Exception exception, string contentType, Guid? faultedMessageId, string[] faultMessageTypes)
        {
            Timestamp = DateTime.UtcNow;
            FaultId = NewId.NextGuid();

            Host = host;
            ContentType = contentType;
            FaultedMessageId = faultedMessageId;
            FaultMessageTypes = faultMessageTypes;

            var aggregateException = exception as AggregateException;
            Exceptions = aggregateException?.InnerExceptions.Select(x => ((ExceptionInfo)new FaultExceptionInfo(x))).ToArray()
                ?? new ExceptionInfo[] {new FaultExceptionInfo(exception)};
        }

        public Guid FaultId { get; }
        public DateTime Timestamp { get; }
        public Guid? FaultedMessageId { get; }
        public ExceptionInfo[] Exceptions { get; }
        public HostInfo Host { get; }
        public string[] FaultMessageTypes { get; }
        public string ContentType { get; }
    }
}
