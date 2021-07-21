namespace MassTransit.Events
{
    using System;
    using System.Linq;


    [Serializable]
    public class ReceiveFaultEvent :
        ReceiveFault
    {
        protected ReceiveFaultEvent()
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
                ?? new ExceptionInfo[] {new FaultExceptionInfo(exception)};
        }

        public Guid FaultId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid? FaultedMessageId { get; private set; }
        public ExceptionInfo[] Exceptions { get; private set; }
        public HostInfo Host { get; private set; }
        public string[] FaultMessageTypes { get; private set; }
        public string ContentType { get; private set; }
    }
}
