namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public static class CancelScheduledSendExtensions
    {
        /// <summary>
        /// Cancel a scheduled message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="scheduler">The message scheduler</param>
        /// <param name="message">The </param>
        /// <returns></returns>
        public static Task CancelScheduledSend<T>(this IMessageScheduler scheduler, ScheduledMessage<T> message)
            where T : class
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return scheduler.CancelScheduledSend(message.Destination, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message scheduler</param>
        /// <param name="message">The </param>
        /// <returns></returns>
        public static Task CancelScheduledSend<T>(this ConsumeContext context, ScheduledMessage<T> message)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.CancelScheduledSend(message.Destination, message.TokenId);
        }
    }
}
