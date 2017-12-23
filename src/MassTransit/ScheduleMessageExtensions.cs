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
    using GreenPipes;
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
        /// <returns>A handled to the scheduled message</returns>
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message)
            where T : class
        {
            return publishEndpoint.ScheduleSend(destinationAddress, scheduledTime, message);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message, IPipe<SendContext<T>> pipe)
            where T : class
        {
            return publishEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime,
            T message, IPipe<SendContext> pipe)
            where T : class
        {
            return publishEndpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published and delivered</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message)
            where T : class
        {
            return bus.ScheduleSend(bus.Address, scheduledTime, message);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<SendContext<T>> pipe)
            where T : class
        {
            return bus.ScheduleSend(bus.Address, scheduledTime, message, pipe);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe)
            where T : class
        {
            return bus.ScheduleSend(bus.Address, scheduledTime, message, pipe);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(destinationAddress, scheduledTime, message);
            }

            IPublishEndpoint endpoint = context;

            return endpoint.ScheduleSend(destinationAddress, scheduledTime, message);
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
        [Obsolete("Use ScheduleSend instead, it's the future")]
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this ConsumeContext context, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<SendContext> pipe)
            where T : class
        {
            MessageSchedulerContext schedulerContext;
            if (context.TryGetPayload(out schedulerContext))
            {
                return schedulerContext.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
            }

            IPublishEndpoint endpoint = context;

            return endpoint.ScheduleSend(destinationAddress, scheduledTime, message, pipe);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="message"> </param>
        [Obsolete("Use CancelScheduledSend() instead")]
        public static Task CancelScheduledMessage<T>(this IPublishEndpoint publishEndpoint, ScheduledMessage<T> message)
            where T : class
        {
            return publishEndpoint.CancelScheduledSend(message);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        [Obsolete("Use CancelScheduledSend() instead")]
        public static Task CancelScheduledMessage(this IPublishEndpoint publishEndpoint, Guid tokenId)
        {
            return publishEndpoint.CancelScheduledSend(tokenId);
        }
    }
}