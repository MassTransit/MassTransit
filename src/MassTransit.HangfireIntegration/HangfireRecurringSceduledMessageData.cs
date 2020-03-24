namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Scheduling;


    class HangfireRecurringScheduledMessageData : HangfireScheduledMessageData
    {
        public string JobKey { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        public static HangfireRecurringScheduledMessageData Create(ConsumeContext<ScheduleRecurringMessage> context, string jobKey)
        {
            var message = new HangfireRecurringScheduledMessageData
            {
                DestinationAddress = context.Message.Destination?.ToString() ?? "",
                ContentType = context.ReceiveContext.ContentType?.MediaType,
                Body = ExtractBody(context.ReceiveContext.ContentType?.MediaType, context.ReceiveContext.GetBody(), context.Message.Destination),
                FaultAddress = context.FaultAddress?.ToString() ?? "",
                ResponseAddress = context.ResponseAddress?.ToString() ?? "",
                JobKey = jobKey,
                StartTime = context.Message.Schedule.StartTime,
                EndTime = context.Message.Schedule.EndTime
            };

            if (context.MessageId.HasValue)
                message.MessageId = context.MessageId.Value.ToString();

            if (context.CorrelationId.HasValue)
                message.CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                message.ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                message.InitiatorId = context.InitiatorId.Value.ToString();

            if (context.RequestId.HasValue)
                message.RequestId = context.RequestId.Value.ToString();

            if (context.ExpirationTime.HasValue)
                message.ExpirationTime = context.ExpirationTime.Value.ToString("O");

            if (!string.IsNullOrEmpty(jobKey))
                message.JobKey = jobKey;

            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll();
            if (headers.Any())
                message.HeadersAsJson = JsonConvert.SerializeObject(headers);

            return message;
        }
    }
}
