namespace MassTransit.ActiveMqTransport
{
    using System;
    using Apache.NMS;


    public static class ActiveMqSendContextExtensions
    {
        /// <summary>
        /// Sets the priority of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static void SetPriority(this SendContext context, MsgPriority priority)
        {
            if (!context.TryGetPayload(out ActiveMqSendContext sendContext))
                throw new ArgumentException("The ActiveMqSendContext was not available");

            sendContext.Priority = priority;
        }

        /// <summary>
        /// Sets the priority of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static bool TrySetPriority(this SendContext context, MsgPriority priority)
        {
            if (!context.TryGetPayload(out ActiveMqSendContext sendContext))
                return false;

            sendContext.Priority = priority;
            return true;
        }
    }
}
