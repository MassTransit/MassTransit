namespace MassTransit.Scheduling
{
    using System;
    using Metadata;


    public class ScheduleMessageCommand<T> :
        ScheduleMessage<T>
        where T : class
    {
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

        public Guid CorrelationId { get; }
        public DateTime ScheduledTime { get; }
        public string[] PayloadType { get; }
        public Uri Destination { get; }
        public T Payload { get; }
    }
}
