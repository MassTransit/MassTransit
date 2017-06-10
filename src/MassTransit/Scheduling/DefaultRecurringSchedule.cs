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
namespace MassTransit.Scheduling
{
    using System;
    using System.Reflection;
    using Util;


    public abstract class DefaultRecurringSchedule : RecurringSchedule
    {
        protected DefaultRecurringSchedule()
        {
            ScheduleId = TypeMetadataCache.GetShortName(GetType());
            ScheduleGroup = GetType().GetTypeInfo().Assembly.FullName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];

            TimeZoneId = TimeZoneInfo.Local.Id;
            StartTime = DateTime.Now;
        }

        public MissedEventPolicy MisfirePolicy { get; protected set; }
        public string TimeZoneId { get; protected set; }
        public DateTimeOffset StartTime { get; protected set; }
        public DateTimeOffset? EndTime { get; protected set; }
        public string ScheduleId { get; private set; }
        public string ScheduleGroup { get; private set; }
        public string CronExpression { get; protected set; }
        public string Description { get; protected set; }
    }
}