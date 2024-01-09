#nullable enable
namespace MassTransit
{
    using System;


    public static class SqlSendContextExtensions
    {
        /// <summary>
        /// Sets the message priority (default: 100)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static void SetPriority(this SendContext context, short priority)
        {
            if (!context.TryGetPayload(out SqlSendContext? sendContext))
                throw new ArgumentException("The DbSendContext was not available");

            sendContext.Priority = priority == 100 ? default(short?) : priority;
        }

        /// <summary>
        /// Sets the message priority (default: 100)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static bool TrySetPriority(this SendContext context, short priority)
        {
            if (!context.TryGetPayload(out SqlSendContext? sendContext))
                return false;

            sendContext.Priority = priority == 100 ? default(short?) : priority;
            return true;
        }
    }
}
