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
    using Scheduling;


    public static class SendEndpointRecurringSchedulerExtensions
    {
        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this ISendEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this ISendEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, Type messageType, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, messageType, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this ISendEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="message">The message object</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(this ISendEndpoint endpoint, Uri destinationAddress, RecurringSchedule schedule,
            object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, message, messageType, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend<T>(destinationAddress, schedule, values, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend(destinationAddress, schedule, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Schedule a message for recurring delivery using the specified schedule
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The message scheduler endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The schedule for the message to be delivered</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.ScheduleRecurringSend<T>(destinationAddress, schedule, values, pipe, cancellationToken);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="message">The schedule message reference</param>
        public static Task CancelScheduledRecurringSend<T>(this ISendEndpoint endpoint, ScheduledRecurringMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledRecurringSend(endpoint, message.Schedule.ScheduleId, message.Schedule.ScheduleGroup);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="scheduleId">The scheduleId from the recurring schedule</param>
        /// <param name="scheduleGroup">The scheduleGroup from the recurring schedule</param>
        public static Task CancelScheduledRecurringSend(this ISendEndpoint endpoint, string scheduleId, string scheduleGroup)
        {
            IRecurringMessageScheduler scheduler = new EndpointRecurringMessageScheduler(endpoint);

            return scheduler.CancelScheduledRecurringSend(scheduleId, scheduleGroup);
        }
    }
}
