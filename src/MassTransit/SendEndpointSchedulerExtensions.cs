namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Scheduling;


    public static class SendEndpointSchedulerExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default)
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, CancellationToken cancellationToken = default)
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));

            return scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object message,
            CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object message,
            Type messageType, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object values,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend<T>(endpoint, destinationAddress, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(endpoint, destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress, TimeSpan delay, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend<T>(endpoint, destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="message">The schedule message reference</param>
        public static Task CancelScheduledSend<T>(this ISendEndpoint endpoint, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledSend(endpoint, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static Task CancelScheduledSend(this ISendEndpoint endpoint, Guid tokenId)
        {
            IMessageScheduler scheduler = new MessageScheduler(new EndpointScheduleMessageProvider(() => Task.FromResult(endpoint)));


            return scheduler.CancelScheduledSend(tokenId);
        }
    }
}
