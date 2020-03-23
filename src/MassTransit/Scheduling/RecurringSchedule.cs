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


    public interface RecurringSchedule
    {
        /// <summary>
        /// The timezone of the schedule
        /// </summary>
        string TimeZoneId { get; }

        /// <summary>
        /// The time the recurring schedule is enabled
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        /// The time the recurring schedule is disabled
        /// If null then the job is repeated forever
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        /// A unique name that identifies this schedule.
        /// </summary>
        string ScheduleId { get; }

        /// <summary>
        /// A
        /// </summary>
        string ScheduleGroup { get; }

        /// <summary>
        /// The Cron Schedule Expression in Cron Syntax
        /// </summary>
        string CronExpression { get; }

        /// <summary>
        /// Schedule description
        /// </summary>
        string Description { get; }

        MissedEventPolicy MisfirePolicy { get; }
    }
}
