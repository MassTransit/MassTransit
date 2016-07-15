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
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;


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
        /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(publishEndpoint);

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="publishEndpoint">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message, IPipe<SendContext<T>> pipe)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(publishEndpoint);

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="publishEndpoint">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message, IPipe<SendContext> pipe)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(publishEndpoint);

            return scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(bus);

            return scheduler.ScheduleSend(bus.Address, scheduledTime, message);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<SendContext<T>> pipe)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(bus);

            return scheduler.ScheduleSend(bus.Address, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="pipe">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe)
            where T : class
        {
            IMessageScheduler scheduler = new PublishMessageScheduler(bus);

            return scheduler.ScheduleSend(bus.Address, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="context">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, DateTime scheduledTime, T message)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(scheduledTime, message);
            }

            return ScheduleMessage(context, context.ReceiveContext.InputAddress, scheduledTime, message);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="context">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="pipe">A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(scheduledTime, message, pipe);
            }

            return ScheduleMessage(context, context.ReceiveContext.InputAddress, scheduledTime, message, pipe);
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
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(destinationAddress, scheduledTime, message);
            }

            return ScheduleMessage((IPublishEndpoint)context, destinationAddress, scheduledTime, message);
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
        /// <param name="pipe">A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
            }

            return ScheduleMessage((IPublishEndpoint)context, destinationAddress, scheduledTime, message, pipe);
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
        public static Task CancelScheduledMessage(this IPublishEndpoint publishEndpoint, Guid tokenId)
        {
            if (publishEndpoint == null)
                throw new ArgumentNullException(nameof(publishEndpoint));

            IMessageScheduler scheduler = new PublishMessageScheduler(publishEndpoint);

            return scheduler.CancelScheduledSend(tokenId);
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
    }
}