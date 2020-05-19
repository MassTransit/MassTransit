namespace MassTransit.HangfireIntegration
{
    using System;
    using Scheduling;


    class HangfireRecurringScheduledMessageData :
        HangfireScheduledMessageData
    {
        public string JobKey { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        public static HangfireRecurringScheduledMessageData Create(ConsumeContext<ScheduleRecurringMessage> context, string jobKey)
        {
            var message = new HangfireRecurringScheduledMessageData
            {
                JobKey = jobKey,
                StartTime = context.Message.Schedule.StartTime,
                EndTime = context.Message.Schedule.EndTime
            };

            SetBaseProperties(message, context, context.Message.Destination);

            return message;
        }
    }
}
