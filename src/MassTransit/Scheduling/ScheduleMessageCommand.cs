namespace MassTransit.Scheduling
{
    using System;
    using Metadata;


    public class ScheduleMessageCommand<T> :
        ScheduleMessage<T>
        where T : class
    {
        protected ScheduleMessageCommand()
        {
        }

        public ScheduleMessageCommand(DateTime scheduledTime, Uri destination, T payload, Guid tokenId)
        {
            CorrelationId = tokenId;

            ScheduledTime = scheduledTime.Kind == DateTimeKind.Local
                ? scheduledTime.ToUniversalTime()
                : scheduledTime;

            Destination = destination;
            Payload = payload;

            PayloadType = TypeMetadataCache<T>.MessageTypeNames;
        }

        public Guid CorrelationId { get; private set; }
        public DateTime ScheduledTime { get; private set; }
        public string[] PayloadType { get; private set; }
        public Uri Destination { get; private set; }
        public T Payload { get; private set; }
    }


    public class ScheduleMessageCommand :
        ScheduleMessage
    {
        public Guid CorrelationId { get; private set; }
        public DateTime ScheduledTime { get; private set; }
        public string[] PayloadType { get; private set; }
        public Uri Destination { get; private set; }
    }
}
