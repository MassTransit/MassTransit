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


    public static class ScheduleRecurringMessageExtensions
    {
        /// <summary>
        /// Schedules a recurring message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="publishEndpoint">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="schedule">The recurring schedule instance</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        [Obsolete("Use the ScheduleRecurringSend method instead")]
        public static async Task<ScheduledRecurringMessage<T>> ScheduleRecurringMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message, IPipe<PublishContext<ScheduleRecurringMessage<T>>> contextCallback = null)
            where T : class
        {
            var command = new ScheduleRecurringMessageCommand<T>(schedule, destinationAddress, message);

            await publishEndpoint.Publish(command, contextCallback ?? Pipe.Empty<PublishContext<ScheduleRecurringMessage<T>>>()).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(command.Schedule, command.Destination, command.Payload);
        }

        /// <summary>
        ///     Schedules a message to be sent to the bus using a Publish, which should only be used when
        ///     the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published</param>
        /// <param name="schedule">An instance of IRecurringSchedule that defines this schedule</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        [Obsolete("Use the ScheduleRecurringSend method instead")]
        public static Task<ScheduledRecurringMessage<T>> ScheduleRecurringMessage<T>(this IBus bus, RecurringSchedule schedule, T message,
            IPipe<PublishContext<ScheduleRecurringMessage<T>>> contextCallback = null)
            where T : class
        {
            return ScheduleRecurringMessage(bus, bus.Address, schedule, message, contextCallback);
        }

        /// <summary>
        ///     Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="message"> </param>
        [Obsolete("Use the CancelScheduledRecurringSend method instead")]
        public static Task CancelScheduledRecurringMessage<T>(this IPublishEndpoint bus, ScheduledRecurringMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledRecurringMessage(bus, message.Schedule.ScheduleId, message.Schedule.ScheduleGroup);
        }

        /// <summary>
        ///     Cancel a scheduled recurring message using the id and group of the schedule class.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="schedule"></param>
        [Obsolete("Use the CancelScheduledRecurringSend method instead")]
        public static Task CancelScheduledRecurringMessage(this IPublishEndpoint bus, RecurringSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));
            return CancelScheduledRecurringMessage(bus, schedule.ScheduleId, schedule.ScheduleGroup);
        }

        /// <summary>
        ///     Cancel a scheduled recurring message using the id and group of the schedule.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="scheduleId"></param>
        /// <param name="scheduleGroup"></param>
        [Obsolete("Use the CancelScheduledRecurringSend method instead")]
        public static Task CancelScheduledRecurringMessage(this IPublishEndpoint bus, string scheduleId, string scheduleGroup)
        {
            var command = new CancelScheduledRecurringMessageCommand(scheduleId, scheduleGroup);

            return bus.Publish<CancelScheduledRecurringMessage>(command);
        }
    }
}