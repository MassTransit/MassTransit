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
namespace MassTransit.Scheduling
{
    using System;
    using Metadata;
    using Util;


    public class ScheduleRecurringMessageCommand<T> :
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
                $"Group: {Schedule.ScheduleGroup}, Id: {Schedule.ScheduleId}, StartTime: {Schedule.StartTime}, EndTime: {Schedule.EndTime}, CronExpression: {Schedule.CronExpression}, TimeZone: {Schedule.TimeZoneId}";
        }
    }
}