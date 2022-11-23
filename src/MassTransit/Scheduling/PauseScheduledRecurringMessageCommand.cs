namespace MassTransit.Scheduling
{
    using System;


    public class PauseScheduledRecurringMessageCommand :
        PauseScheduledRecurringMessage
    {
        public PauseScheduledRecurringMessageCommand()
        {
        }

        public PauseScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
        {
            CorrelationId = NewId.NextGuid();
            Timestamp = DateTime.UtcNow;

            ScheduleId = scheduleId;
            ScheduleGroup = scheduleGroup;
        }

        public Guid CorrelationId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ScheduleId { get; set; }
        public string ScheduleGroup { get; set; }
    }
}
