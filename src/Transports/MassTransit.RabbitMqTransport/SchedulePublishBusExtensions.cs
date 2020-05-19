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
    using Metadata;
    using Scheduling;
    using Util;


    public static class SchedulePublishBusExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetPublishAddress(bus, messageType);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var destinationAddress = GetPublishAddress(bus, messageType);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            var destinationAddress = GetPublishAddress(bus, messageType);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var destinationAddress = GetPublishAddress(bus, messageType);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object values,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var destinationAddress = GetPublishAddress<T>(bus);

            return schedulerEndpoint.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, T message,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object message, Type messageType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, messageType, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an object as a message, using the message type specified. If the object cannot be cast
        ///     to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage> SchedulePublish(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object values,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish<T>(bus, schedulerEndpoint, scheduledTime, values, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="schedulerEndpoint">The scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object values,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish(bus, schedulerEndpoint, scheduledTime, values, pipe, cancellationToken);
        }

        /// <summary>
        ///     Sends an interface message, initializing the properties of the interface using the anonymous
        ///     object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="bus">The consume context</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="schedulerEndpoint"></param>
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this IBus bus, ISendEndpoint schedulerEndpoint, TimeSpan delay, object values,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + delay;

            return SchedulePublish<T>(bus, schedulerEndpoint, scheduledTime, values, pipe, cancellationToken);
        }

        static Uri GetPublishAddress<T>(IBus bus)
            where T : class
        {
            if (bus.Topology.Publish<T>().TryGetPublishAddress(bus.Address, out var address))
                return address;

            throw new ArgumentException($"The publish address for the specified type was not returned: {TypeMetadataCache<T>.ShortName}");
        }

        static Uri GetPublishAddress(IBus bus, Type messageType)
        {
            if (bus.Topology.PublishTopology.TryGetPublishAddress(messageType, bus.Address, out var address))
                return address;

            throw new ArgumentException($"The publish address for the specified type was not returned: {TypeMetadataCache.GetShortName(messageType)}");
        }
    }
}