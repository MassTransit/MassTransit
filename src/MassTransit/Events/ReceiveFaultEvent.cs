namespace MassTransit.Events
{
    using System;
    using System.Linq;


    [Serializable]
    public class ReceiveFaultEvent :
        ReceiveFault
    {
        public ReceiveFaultEvent()
        {
        }

        public ReceiveFaultEvent(HostInfo host, Exception exception, string contentType, Guid? faultedMessageId, string[] faultMessageTypes)
        {
            Timestamp = DateTime.UtcNow;
            FaultId = NewId.NextGuid();

            Host = host;
            ContentType = contentType;
            FaultedMessageId = faultedMessageId;
            FaultMessageTypes = faultMessageTypes;

            var aggregateException = exception as AggregateException;
            Exceptions = aggregateException?.InnerExceptions.Select(x => (ExceptionInfo)new FaultExceptionInfo(x)).ToArray()
                ?? new ExceptionInfo[] { new FaultExceptionInfo(exception) };
        }

        public Guid FaultId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? FaultedMessageId { get; set; }
        public ExceptionInfo[] Exceptions { get; set; }
        public HostInfo Host { get; set; }
        public string[] FaultMessageTypes { get; set; }
        public string ContentType { get; set; }
    }
}
