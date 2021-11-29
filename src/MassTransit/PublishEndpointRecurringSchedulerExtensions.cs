namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Scheduling;


    public static class PublishEndpointRecurringSchedulerExtensions
    {
        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, CancellationToken cancellationToken = default)
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, Type messageType, CancellationToken cancellationToken = default)
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, messageType, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this IPublishEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend<T>(destinationAddress, schedule, values, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this IPublishEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend<T>(destinationAddress, schedule, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="message">The schedule message reference</param>
        public static Task CancelScheduledRecurringSend<T>(this IPublishEndpoint endpoint, ScheduledRecurringMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledRecurringSend(endpoint, message.Schedule.ScheduleId, message.Schedule.ScheduleGroup);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduleId and scheduleGroup that was returned when the message was scheduled.
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="scheduleId">The scheduleId from the recurring schedule</param>
        /// <param name="scheduleGroup">The scheduleGroup from the recurring schedule</param>
        public static Task CancelScheduledRecurringSend(this IPublishEndpoint endpoint, string scheduleId, string scheduleGroup)
        {
            IRecurringMessageScheduler scheduler = new PublishRecurringMessageScheduler(endpoint);

            return scheduler.CancelScheduledRecurringSend(scheduleId, scheduleGroup);
        }
    }
}
