// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;


    /// <summary>
    ///     Extensions for scheduling publish/send message
    /// </summary>
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
        public static async Task<IRecurringMessage<T>> ScheduleRecurringMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, IRecurringSchedule schedule, T message, IPipe<PublishContext<IScheduleRecurringMessage<T>>> contextCallback = null)
            where T : class
        {
            var command = new ScheduleRecurringMessageCommand<T>(schedule, destinationAddress, message);

            await publishEndpoint.Publish(command, contextCallback ?? Pipe.Empty<PublishContext<IScheduleRecurringMessage<T>>>());

            return new ScheduledRecurringMessageHandle<T>(command.ScheduleId,command.ScheduleGroup, command.StartTime, command.EndTime,command.CronExpression,command.MisfirePolicy, command.Destination, command.Payload, command.TimeZoneId);
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
        public static Task<IRecurringMessage<T>> ScheduleRecurringMessage<T>(this IBus bus, IRecurringSchedule schedule, T message, IPipe<PublishContext<IScheduleRecurringMessage<T>>> contextCallback = null)
            where T : class
        {
            return ScheduleRecurringMessage(bus, bus.Address, schedule, message, contextCallback);
        }

       

        /// <summary>
        ///     Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="message"> </param>
        public static Task CancelScheduledRecurringMessage<T>(this IPublishEndpoint bus, IRecurringMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return  CancelScheduledRecurringMessage(bus, message.ScheduleId,message.ScheduleGroup);
        }

        /// <summary>
        ///     Cancel a scheduled recurring message using the id and group of the schedule.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="scheduleId"></param>
        /// <param name="scheduleGroup"></param>
        private static async Task CancelScheduledRecurringMessage(this IPublishEndpoint bus, string scheduleId, string scheduleGroup)
        {

            var command = new CancelScheduledRecurringMessageCommand(scheduleId, scheduleGroup);

            await bus.Publish<ICancelScheduledRecurringMessage>(command);
        
        }

      

        private class CancelScheduledRecurringMessageCommand :
            ICancelScheduledRecurringMessage
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


        private class ScheduleRecurringMessageCommand<T> : IScheduleRecurringMessage<T>
            where T : class
        {
            public override string ToString()
            {
                return string.Format("ScheduleGroup: {0}, ScheduleId: {1}, StartTime: {2}, EndTime: {3}, CronExpression: {4}, TimeZone: {5}", ScheduleGroup, ScheduleId, StartTime, EndTime, CronExpression, TimeZoneId);
            }

            public ScheduleRecurringMessageCommand(IRecurringSchedule schedule, Uri destination, T payload)
            {
                
                CorrelationId = NewId.NextGuid();

                StartTime =  schedule.StartTime;
                EndTime = schedule.EndTime;
                TimeZoneId = schedule.TimeZoneId;

                ScheduleId = schedule.ScheduleId;
                ScheduleGroup = schedule.ScheduleGroup;
                CronExpression = schedule.CronExpression;
                MisfirePolicy = schedule.MisfirePolicy;

                Destination = destination;
                Payload = payload;

                PayloadType = typeof (T).GetMessageTypes()
                    .Select(x => new MessageUrn(x).ToString())
                    .ToList();
            }

            public Guid CorrelationId { get; private set; }
         
            public IList<string> PayloadType { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
            public string TimeZoneId { get; private set; }
            public DateTimeOffset StartTime { get; private set; }
            public DateTimeOffset? EndTime { get; private set; }
            public string ScheduleId { get; private set; }
            public string ScheduleGroup { get; private set; }
            public string CronExpression { get; private set; }
            public MisfireInstruction MisfirePolicy { get; private set; }
        }


        class ScheduledRecurringMessageHandle<T> : IRecurringMessage<T>
            where T : class
        {
            public ScheduledRecurringMessageHandle(string scheduleId, string scheduleGroup, DateTimeOffset startTime, DateTimeOffset? endTime, string cronExpression, MisfireInstruction misfirePolicy, Uri destination, T payload, string timeZoneId)
            {
                ScheduleId = scheduleId;
                ScheduleGroup = scheduleGroup;
                StartTime = startTime;
                EndTime = endTime;
                CronExpression = cronExpression;
                Destination = destination;
                Payload = payload;
                MisfirePolicy = misfirePolicy;
                TimeZoneId = timeZoneId;
            }

            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
            public string TimeZoneId { get; private set; }
            public DateTimeOffset StartTime { get; private set; }
            public DateTimeOffset? EndTime { get; private set; }
            public string ScheduleId { get; private set; }
            public string ScheduleGroup { get; private set; }
            public string CronExpression { get; private set; }
            public MisfireInstruction MisfirePolicy { get; private set; }
        }
    
    }
}