namespace MassTransit
{
    using System;


    public static class ServiceBusSendContextExtensions
    {
        /// <summary>
        /// Set the time at which the message should be delivered to the queue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduledTime">The scheduled time for the message</param>
        public static void SetScheduledEnqueueTime(this SendContext context, DateTime scheduledTime)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                if (scheduledTime.Kind == DateTimeKind.Local)
                    scheduledTime = scheduledTime.ToUniversalTime();

                sendContext.ScheduledEnqueueTimeUtc = scheduledTime;
            }
        }

        /// <summary>
        /// Set the time at which the message should be delivered to the queue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delay">The time to wait before the message should be enqueued</param>
        public static void SetScheduledEnqueueTime(this SendContext context, TimeSpan delay)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
                sendContext.ScheduledEnqueueTimeUtc = DateTime.UtcNow + delay;
        }

        public static void SetSessionId(this SendContext context, string sessionId)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
                sendContext.SessionId = sessionId;
        }

        public static void SetReplyToSessionId(this SendContext context, string sessionId)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
                sendContext.ReplyToSessionId = sessionId;
        }

        public static void SetPartitionKey(this SendContext context, string partitionKey)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
                sendContext.PartitionKey = partitionKey;
        }
    }
}
