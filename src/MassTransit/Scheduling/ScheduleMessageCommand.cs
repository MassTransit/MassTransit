namespace MassTransit.Scheduling
{
    using System;


    [Serializable]
    public class ScheduleMessageCommand<T> :
        ScheduleMessage
        where T : class
    {
        public ScheduleMessageCommand()
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

            PayloadType = MessageTypeCache<T>.MessageTypeNames;
        }

        public Guid CorrelationId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string[] PayloadType { get; set; }
        public Uri Destination { get; set; }
        public object Payload { get; set; }
    }


    [Serializable]
    public class ScheduleMessageCommand :
        ScheduleMessage
    {
        public Guid CorrelationId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string[] PayloadType { get; set; }
        public Uri Destination { get; set; }
        public object Payload { get; set; }
    }
}
