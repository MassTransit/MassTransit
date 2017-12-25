// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMqTransport;
    using Scheduling;
    using Util;


    public static class SchedulePublishExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetDestinationAddress(context, message, messageType);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var destinationAddress = GetDestinationAddress(context, message, messageType);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetDestinationAddress(context, message, messageType);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var destinationAddress = GetDestinationAddress(context, message, messageType);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, object values,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var destinationAddress = GetDestinationAddress(context, message);

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, T message,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, TimeSpan delay, object message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, TimeSpan delay, object message, Type messageType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, TimeSpan delay, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="context">The consume context</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this ConsumeContext context, TimeSpan delay, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, object values,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish<T>(context, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(context, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="context">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish<T>(context, scheduledTime, values, pipe, cancellationToken);
        }

        static Uri GetDestinationAddress<T>(ConsumeContext context, T message)
            where T : class
        {
            var modelContext = context.ReceiveContext.GetPayload<ModelContext>();

            Uri destinationAddress;
            if (modelContext.PublishTopology.GetMessageTopology<T>().TryGetPublishAddress(modelContext.ConnectionContext.HostAddress, out destinationAddress))
                return destinationAddress;

            throw new ArgumentException($"The publish address for the message was not found: {TypeMetadataCache<T>.ShortName}", nameof(message));
        }
        static Uri GetDestinationAddress(ConsumeContext context, object message, Type messageType)
        {
            var modelContext = context.ReceiveContext.GetPayload<ModelContext>();

            // TODO Implement object redirector
            throw new NotImplementedException("This needs to be done yet");
        }

    }
}