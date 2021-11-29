namespace MassTransit.Scheduling
{
    using System;


    public class CancelScheduledRecurringMessageCommand :
        CancelScheduledRecurringMessage
    {
        public CancelScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
        {
            CorrelationId = NewId.NextGuid();
            Timestamp = DateTime.UtcNow;

            ScheduleId = scheduleId;
            ScheduleGroup = scheduleGroup;
        }

        public Guid CorrelationId { get; }
        public DateTime Timestamp { get; }
        public string ScheduleId { get; }
        public string ScheduleGroup { get; }
    }
}
