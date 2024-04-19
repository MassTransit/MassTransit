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
        [Test]
        [Explicit]
        public async Task Should_cancel_recurring_schedule()
        {
            var scheduleId = Guid.NewGuid().ToString();

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new Done { Name = "Joe" });
            ScheduledRecurringMessage<Interval> scheduledRecurringMessage =
                await HangfireEndpoint.ScheduleRecurringSend(InputQueueAddress, new MyCancelableSchedule(scheduleId), new Interval { Name = "Joe" });

            await _done;

            var countBeforeCancel = _count;
            Assert.That(_count, Is.EqualTo(8), "Expected to see 8 interval messages");

            await Bus.CancelScheduledRecurringSend(scheduledRecurringMessage);

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new DoneAgain { Name = "Joe" });

            await _doneAgain;

            Assert.That(_count, Is.EqualTo(countBeforeCancel), "Expected to see the count matches.");
        }

        [Test]
        [Explicit]
        public async Task Should_handle_now_properly()
        {
            var scheduleId = NewId.NextGuid().ToString("N");
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(20), new Done { Name = "Joe" });
            await HangfireEndpoint.ScheduleRecurringSend(InputQueueAddress, new MySchedule(scheduleId), new Interval { Name = "Joe" });

            await _done;

            Assert.That(_count, Is.EqualTo(8), "Expected to see 8 interval messages");
        }

        [Test]
        [Explicit]
        public async Task Should_pause_recurring_schedule()
        {
            var scheduleId = Guid.NewGuid().ToString();

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new Done { Name = "Joe" });
            ScheduledRecurringMessage<Interval> scheduledRecurringMessage =
                await HangfireEndpoint.ScheduleRecurringSend(InputQueueAddress, new MyCancelableSchedule(scheduleId), new Interval { Name = "Joe" });

            await _done;

            var countBeforeCancel = _count;
            Assert.That(_count, Is.EqualTo(8), "Expected to see 8 interval messages");

            await Bus.PauseScheduledRecurringSend(scheduledRecurringMessage);

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(11), new DoneAgain { Name = "Joe" });

            await _doneAgain;

            Assert.That(_count, Is.EqualTo(countBeforeCancel), "Expected to see the count matches.");
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
