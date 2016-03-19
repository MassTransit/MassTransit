// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;
    using Util;


    /// <summary>
    /// Extensions for scheduling publish/send message 
    /// </summary>
    public static class ScheduleMessageExtensions
    {
        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="publishEndpoint">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static async Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message, IPipe<PublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            var command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message);

            await publishEndpoint.Publish(command, contextCallback ?? Pipe.Empty<PublishContext<ScheduleMessage<T>>>()).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(command.CorrelationId, command.ScheduledTime, command.Destination,
                command.Payload);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<PublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            return ScheduleMessage(bus, bus.Address, scheduledTime, message, contextCallback ?? Pipe.Empty<PublishContext<ScheduleMessage<T>>>());
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="context">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="sendPipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, DateTime scheduledTime, T message,
            IPipe<SendContext> sendPipe = null)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(scheduledTime, message, sendPipe ?? Pipe.Empty<SendContext>());
            }

            return ScheduleMessage(context, context.ReceiveContext.InputAddress, scheduledTime, message, sendPipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="context">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="destinationAddress"></param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext> contextCallback = null)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(destinationAddress, scheduledTime, message, contextCallback ?? Pipe.Empty<SendContext>());
            }

            return ScheduleMessage((IPublishEndpoint)context, destinationAddress, scheduledTime, message, contextCallback);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="message"> </param>
        public static Task CancelScheduledMessage<T>(this IPublishEndpoint publishEndpoint, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledMessage(publishEndpoint, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static async Task CancelScheduledMessage(this IPublishEndpoint publishEndpoint, Guid tokenId)
        {
            if (publishEndpoint == null)
                throw new ArgumentNullException(nameof(publishEndpoint));
            var command = new CancelScheduledMessageCommand(tokenId);

            await publishEndpoint.Publish<CancelScheduledMessage>(command).ConfigureAwait(false);
        }


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

            return scheduler.CancelScheduledSend(message.TokenId);
        }



        class CancelScheduledMessageCommand :
            CancelScheduledMessage
        {
            public CancelScheduledMessageCommand(Guid tokenId)
            {
                CorrelationId = NewId.NextGuid();
                Timestamp = DateTime.UtcNow;

                TokenId = tokenId;
            }

            public Guid TokenId { get; }
            public DateTime Timestamp { get; }
            public Guid CorrelationId { get; }
        }


        class ScheduleMessageCommand<T> :
            ScheduleMessage<T>
            where T : class
        {
            public ScheduleMessageCommand(DateTime scheduledTime, Uri destination, T payload)
            {
                CorrelationId = NewId.NextGuid();

                ScheduledTime = scheduledTime.Kind == DateTimeKind.Local
                    ? scheduledTime.ToUniversalTime()
                    : scheduledTime;

                Destination = destination;
                Payload = payload;

                PayloadType = TypeMetadataCache<T>.MessageTypeNames;
            }

            public Guid CorrelationId { get; }
            public DateTime ScheduledTime { get; }
            public string[] PayloadType { get; }
            public Uri Destination { get; }
            public T Payload { get; }
        }


        class ScheduledMessageHandle<T> :
            ScheduledMessage<T>
            where T : class
        {
            public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
            {
                TokenId = tokenId;
                ScheduledTime = scheduledTime;
                Destination = destination;
                Payload = payload;
            }

            public Guid TokenId { get; }
            public DateTime ScheduledTime { get; }
            public Uri Destination { get; }
            public T Payload { get; }
        }
    }
}