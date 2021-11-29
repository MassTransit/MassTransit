namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Scheduling;


    /// <summary>
    /// A message scheduler is able to schedule a message for delivery.
    /// </summary>
    public interface IRecurringMessageScheduler
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message, Type messageType,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values,
            IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Cancel a scheduled message by TokenId
        /// </summary>
        /// <param name="scheduleId">The scheduleId from the recurring schedule</param>
        /// <param name="scheduleGroup">The scheduleGroup from the recurring schedule</param>
        Task CancelScheduledRecurringSend(string scheduleId, string scheduleGroup);
    }
}
