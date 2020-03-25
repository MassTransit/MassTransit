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
namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scheduling;


    [TestFixture]
    public class Specifying_a_recurring_event :
        HangfireInMemoryTestFixture
    {
        [Test, Explicit]
        public async Task Should_handle_now_properly()
        {
            var scheduleId = NewId.NextGuid().ToString("N");
            await HangfireEndpoint.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(20), new Done {Name = "Joe"});
            await HangfireEndpoint.ScheduleRecurringSend(InputQueueAddress, new MySchedule(scheduleId), new Interval {Name = "Joe"});

            await _done;

            Assert.AreEqual(8, _count, "Expected to see 8 interval messages");
        }

        [Test, Explicit]
        public async Task Should_cancel_recurring_schedule()
        {
            var scheduleId = Guid.NewGuid().ToString();

            await HangfireEndpoint.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new Done {Name = "Joe"});
            var scheduledRecurringMessage =
                await HangfireEndpoint.ScheduleRecurringSend(InputQueueAddress, new MyCancelableSchedule(scheduleId), new Interval {Name = "Joe"});

            await _done;

            var countBeforeCancel = _count;
            Assert.AreEqual(8, _count, "Expected to see 8 interval messages");

            await Bus.CancelScheduledRecurringSend(scheduledRecurringMessage);

            await HangfireEndpoint.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new DoneAgain {Name = "Joe"});

            await _doneAgain;

            Assert.AreEqual(countBeforeCancel, _count, "Expected to see the count matches.");
        }

        Task<ConsumeContext<Done>> _done;
        Task<ConsumeContext<DoneAgain>> _doneAgain;
        int _count;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;
            configurator.Handler<Interval>(async context =>
            {
                Interlocked.Increment(ref _count);
            });

            _done = Handled<Done>(configurator);
            _doneAgain = Handled<DoneAgain>(configurator);
        }


        class MySchedule :
            RecurringSchedule
        {
            public MySchedule(string scheduleId)
            {
                ScheduleId = scheduleId;
                CronExpression = "0/1 * * * * ?";

                StartTime = DateTime.Now + TimeSpan.FromSeconds(3);
                EndTime = StartTime + TimeSpan.FromSeconds(8);
                Description = "my description";
            }

            public string TimeZoneId { get; private set; }
            public DateTimeOffset StartTime { get; private set; }
            public DateTimeOffset? EndTime { get; private set; }
            public string ScheduleId { get; private set; }
            public string ScheduleGroup { get; private set; }
            public string CronExpression { get; private set; }
            public string Description { get; private set; }
            public MissedEventPolicy MisfirePolicy { get; private set; }
        }


        class MyCancelableSchedule :
            RecurringSchedule
        {
            public MyCancelableSchedule(string scheduleId)
            {
                ScheduleId = scheduleId;
                CronExpression = "0/1 * * * * ?";

                StartTime = DateTime.Now + TimeSpan.FromSeconds(3);
                EndTime = StartTime + TimeSpan.FromSeconds(20);
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


        public class Interval
        {
            public string Name { get; set; }
        }


        public class Done
        {
            public string Name { get; set; }
        }


        public class DoneAgain
        {
            public string Name { get; set; }
        }
    }
}
