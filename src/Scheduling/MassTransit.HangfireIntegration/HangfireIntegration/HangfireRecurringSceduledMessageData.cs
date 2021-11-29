namespace MassTransit.HangfireIntegration
{
    using System;
    using Context;
    using Scheduling;


    class HangfireRecurringScheduledMessageData :
        HangfireScheduledMessageData
    {
        public string? JobKey { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        public static HangfireRecurringScheduledMessageData Create(ConsumeContext<ScheduleRecurringMessage> context, string jobKey)
        {
            var data = new HangfireRecurringScheduledMessageData
            {
                JobKey = jobKey,
                StartTime = context.Message.Schedule.StartTime,
                EndTime = context.Message.Schedule.EndTime
            };

            var messageBody = context.SerializerContext.GetMessageSerializer(context.Message.Payload, context.Message.PayloadType)
                .GetMessageBody(new MessageSendContext<ScheduleRecurringMessage>(context.Message));

            SetBaseProperties(data, context, context.Message.Destination, messageBody);

            return data;
        }
    }
}
