namespace MassTransit.Scheduling
{
    using System;
    using Metadata;


    public class ScheduleRecurringMessageCommand<T> :
        ScheduleRecurringMessage<T>
        where T : class
    {
        public ScheduleRecurringMessageCommand(RecurringSchedule schedule, Uri destination, T payload)
        {
            CorrelationId = NewId.NextGuid();

            Schedule = schedule;

            Destination = destination;
            Payload = payload;

            PayloadType = TypeMetadataCache<T>.MessageTypeNames;
        }

        public Guid CorrelationId { get; private set; }
        public RecurringSchedule Schedule { get; private set; }
        public string[] PayloadType { get; private set; }
        public Uri Destination { get; private set; }
        public T Payload { get; private set; }

        public override string ToString()
        {
            return
                $"Group: {Schedule.ScheduleGroup}, Id: {Schedule.ScheduleId}, StartTime: {Schedule.StartTime}, EndTime: {Schedule.EndTime}, CronExpression: {Schedule.CronExpression}, TimeZone: {Schedule.TimeZoneId}";
        }
    }
}
