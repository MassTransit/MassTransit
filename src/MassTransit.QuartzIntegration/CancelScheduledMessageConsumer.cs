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
namespace MassTransit.QuartzIntegration
{
    using System.Threading.Tasks;
    using Logging;
    using Quartz;
    using Scheduling;
    using Util;


    public class CancelScheduledMessageConsumer :
        IConsumer<CancelScheduledMessage>,
        IConsumer<CancelScheduledRecurringMessage>
    {
        static readonly ILog _log = Logger.Get<CancelScheduledMessageConsumer>();
        readonly IScheduler _scheduler;

        public CancelScheduledMessageConsumer(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task Consume(ConsumeContext<CancelScheduledMessage> context)
        {
            var correlationId = context.Message.TokenId.ToString("N");

            var jobKey = new JobKey(correlationId);
            var deletedJob = await _scheduler.DeleteJob(jobKey).ConfigureAwait(false);

            if (_log.IsDebugEnabled)
            {
                if (deletedJob)
                    _log.DebugFormat("Cancelled Scheduled Message: {0} at {1}", jobKey, context.Message.Timestamp);
                else
                    _log.DebugFormat("CancelScheduledMessage: no message found {0}", jobKey);
            }
        }

        public async Task Consume(ConsumeContext<CancelScheduledRecurringMessage> context)
        {
            const string prependedValue = "Recurring.Trigger.";

            string scheduleId = context.Message.ScheduleId;

            if (!scheduleId.StartsWith(prependedValue))
            {
                scheduleId = string.Concat(prependedValue, scheduleId);
            }

            bool unscheduledJob = await _scheduler.UnscheduleJob(new TriggerKey(scheduleId, context.Message.ScheduleGroup)).ConfigureAwait(false);

            if (_log.IsDebugEnabled)
            {
                if (unscheduledJob)
                {
                    _log.DebugFormat("CancelRecurringScheduledMessage: {0}/{1} at {2}", context.Message.ScheduleId, context.Message.ScheduleGroup,
                        context.Message.Timestamp);
                }
                else
                    _log.DebugFormat("CancelRecurringScheduledMessage: no message found {0}", context.Message);
            }
        }
    }
}