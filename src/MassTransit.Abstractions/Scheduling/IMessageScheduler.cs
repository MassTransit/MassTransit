namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A message scheduler is able to schedule a message for delivery.
    /// </summary>
    public interface IMessageScheduler
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Cancel a scheduled message by TokenId
        /// </summary>
        /// <param name="destinationAddress">The destination address of the scheduled message</param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledSend(Uri destinationAddress, Guid tokenId);

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> SchedulePublish(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Cancel a scheduled publish, using the tokenId. The message type <typeparamref name="T" /> is used to determine
        /// the destinationAddress.
        /// </summary>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledPublish<T>(Guid tokenId)
            where T : class;

        /// <summary>
        /// Cancel a scheduled publish, using the tokenId. The <paramref name="messageType" /> is used to determine
        /// the destinationAddress.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledPublish(Type messageType, Guid tokenId);
    }
}
