// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Scheduling;
    using GreenPipes;
    using Initializers;
    using Metadata;
    using Scheduling;
    using Util;


    public static class ServiceBusSchedulePublishExtensions
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
            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime);

            return Schedule(context, scheduledTime, message, pipeProxy, cancellationToken);
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
            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime, pipe);

            return Schedule(context, scheduledTime, message, pipeProxy, cancellationToken);
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
            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime, pipe);

            return Schedule(context, scheduledTime, message, pipeProxy, cancellationToken);
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

            var pipeProxy = new ServiceBusScheduleMessagePipe(scheduledTime);

            return Schedule(context, scheduledTime, message, messageType, pipeProxy, cancellationToken);
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
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var pipeProxy = new ServiceBusScheduleMessagePipe(scheduledTime);

            return Schedule(context, scheduledTime, message, messageType, pipeProxy, cancellationToken);
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

            var pipeProxy = new ServiceBusScheduleMessagePipe(scheduledTime, pipe);

            return Schedule(context, scheduledTime, message, messageType, pipeProxy, cancellationToken);
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
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var pipeProxy = new ServiceBusScheduleMessagePipe(scheduledTime, pipe);

            return Schedule(context, scheduledTime, message, messageType, pipeProxy, cancellationToken);
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

            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime);

            return Schedule(context, scheduledTime, values, pipeProxy, cancellationToken);
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

            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime, pipe);

            return Schedule(context, scheduledTime, values, pipeProxy, cancellationToken);
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

            var pipeProxy = new ServiceBusScheduleMessagePipe<T>(scheduledTime, pipe);

            return Schedule(context, scheduledTime, values, pipeProxy, cancellationToken);
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
        /// <param name="delay">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ConsumeContext context, TimeSpan delay, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
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

        static Uri GetDestinationAddress<T>(ConsumeContext context)
            where T : class
        {
            var namespaceContext = context.ReceiveContext.GetPayload<NamespaceContext>();

            if (context.ReceiveContext.PublishTopology.GetMessageTopology<T>().TryGetPublishAddress(namespaceContext.ServiceAddress, out var publishAddress))
            {
                return publishAddress;
            }

            throw new EndpointNotFoundException($"The publish endpoint for the message type could not be found: {TypeMetadataCache<T>.ShortName}");
        }

        static async Task<ScheduledMessage<T>> Schedule<T>(ConsumeContext context, DateTime scheduledTime, T message, ScheduleMessageContextPipe<T> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            await context.Publish(message, pipe, cancellationToken).ConfigureAwait(false);

            var destinationAddress = GetDestinationAddress<T>(context);

            return new ScheduledMessageHandle<T>(pipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, message);
        }

        static async Task<ScheduledMessage<T>> Schedule<T>(ConsumeContext context, DateTime scheduledTime, object values, ScheduleMessageContextPipe<T> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var message = await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            await context.Publish(message, pipe, cancellationToken).ConfigureAwait(false);

            var destinationAddress = GetDestinationAddress<T>(context);

            return new ScheduledMessageHandle<T>(pipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, message);
        }

        static async Task<ScheduledMessage> Schedule(ConsumeContext context, DateTime scheduledTime, object message, Type messageType,
            ScheduleMessageContextPipe pipe, CancellationToken cancellationToken)
        {
            await context.Publish(message, messageType, pipe, cancellationToken).ConfigureAwait(false);

            // TODO this is fucked for now
            var destinationAddress = GetDestinationAddress<object>(context);

            return new ScheduledMessageHandle(pipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress);
        }


        class ScheduledMessageHandle :
            ScheduledMessage
        {
            public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination)
            {
                TokenId = tokenId;
                ScheduledTime = scheduledTime;
                Destination = destination;
            }

            public Guid TokenId { get; }
            public DateTime ScheduledTime { get; }
            public Uri Destination { get; }
        }
    }
}
