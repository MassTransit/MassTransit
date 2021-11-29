namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class ConsumeContextSchedulerExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default)
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, CancellationToken cancellationToken = default)
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object message,
            CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object message,
            Type messageType, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object values,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend<T>(context, destinationAddress, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        /// Sends an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> ScheduleSend(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend(context, destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> ScheduleSend<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return ScheduleSend<T>(context, destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }
    }
}
