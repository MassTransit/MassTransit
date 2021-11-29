namespace MassTransit
{
    using System;


    public static class AmazonSqsSendContextExtensions
    {
        /// <summary>
        /// Sets the GroupId of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="groupId"></param>
        public static void SetGroupId(this SendContext context, string groupId)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                throw new ArgumentException("The AmazonSqsSendContext was not available");

            sendContext.GroupId = groupId;
        }

        /// <summary>
        /// Sets the GroupId of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="groupId"></param>
        public static bool TrySetGroupId(this SendContext context, string groupId)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                return false;

            sendContext.GroupId = groupId;
            return true;
        }

        /// <summary>
        /// Sets the DeduplicationId of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deduplicationId"></param>
        public static void SetDeduplicationId(this SendContext context, string deduplicationId)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                throw new ArgumentException("The AmazonSqsSendContext was not available");

            sendContext.DeduplicationId = deduplicationId;
        }

        /// <summary>
        /// Sets the DeduplicationId of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deduplicationId"></param>
        public static bool TrySetDeduplicationId(this SendContext context, string deduplicationId)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                return false;

            sendContext.DeduplicationId = deduplicationId;
            return true;
        }

        /// <summary>
        /// Sets the DelaySeconds of a message sent to the broker using the TimeSpan (converted to seconds)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delay"></param>
        public static void SetDelay(this SendContext context, TimeSpan delay)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                throw new ArgumentException("The AmazonSqsSendContext was not available");

            sendContext.DelaySeconds = (int)delay.TotalSeconds;
        }

        /// <summary>
        /// Sets the DelaySeconds of a message sent to the broker using the TimeSpan (converted to seconds)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delay"></param>
        public static bool TrySetDelay(this SendContext context, TimeSpan delay)
        {
            if (!context.TryGetPayload(out AmazonSqsSendContext sendContext))
                return false;

            sendContext.DelaySeconds = (int)delay.TotalSeconds;
            return true;
        }
    }
}
