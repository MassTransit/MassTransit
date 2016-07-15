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


    public static class ScheduleRecurringMessageExtensions
    {
        public static async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            RecurringSchedule schedule, T message)
            where T : class
        {
            var command = new ScheduleRecurringMessageCommand<T>(schedule, destinationAddress, message);

            await endpoint.Send<ScheduleRecurringMessage<T>>(command).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(command.Schedule, command.Destination, command.Payload);
        }

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
        /// <param name="scheduleId"></param>
        /// <param name="scheduleGroup"></param>
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
        public static Task CancelScheduledRecurringMessage(this IPublishEndpoint bus, string scheduleId, string scheduleGroup)
        {
            var command = new CancelScheduledRecurringMessageCommand(scheduleId, scheduleGroup);

            return bus.Publish<CancelScheduledRecurringMessage>(command);
        }


        class CancelScheduledRecurringMessageCommand :
            CancelScheduledRecurringMessage
        {
            public CancelScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
            {
                CorrelationId = NewId.NextGuid();
                Timestamp = DateTime.UtcNow;

                ScheduleId = scheduleId;
                ScheduleGroup = scheduleGroup;
            }

            public string ScheduleGroup { get; private set; }

            public string ScheduleId { get; private set; }
            public DateTime Timestamp { get; private set; }
            public Guid CorrelationId { get; private set; }
        }


        class ScheduleRecurringMessageCommand<T> :
            ScheduleRecurringMessage<T>
            where T : class
        {
            public ScheduleRecurringMessageCommand(RecurringSchedule schedule, Uri destination, T payload)
            {
                CorrelationId = NewId.NextGuid();

                Schedule = schedule;

                Destination = destination;
                Payload = payload;

                PayloadType = TypeMetadataCache<T>.MessageTypeNames;
            }

            public Guid CorrelationId { get; private set; }
            public RecurringSchedule Schedule { get; private set; }
            public string[] PayloadType { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }

            public override string ToString()
            {
                return
                    $"ScheduleGroup: {Schedule.ScheduleGroup}, ScheduleId: {Schedule.ScheduleId}, StartTime: {Schedule.StartTime}, EndTime: {Schedule.EndTime}, CronExpression: {Schedule.CronExpression}, TimeZone: {Schedule.TimeZoneId}";
            }
        }


        class ScheduledRecurringMessageHandle<T> :
            ScheduledRecurringMessage<T>
            where T : class
        {
            public ScheduledRecurringMessageHandle(RecurringSchedule schedule, Uri destination, T payload)
            {
                Schedule = schedule;
                Destination = destination;
                Payload = payload;
            }

            public RecurringSchedule Schedule { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
        }
    }
}
