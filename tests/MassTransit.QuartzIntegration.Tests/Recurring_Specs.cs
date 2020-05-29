namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scheduling;


    [TestFixture]
    public class Specifying_a_recurring_event :
        QuartzInMemoryTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_cancel_recurring_schedule()
        {
            var scheduleId = Guid.NewGuid().ToString();

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(10), new Done {Name = "Joe"});
            ScheduledRecurringMessage<Interval> scheduledRecurringMessage =
                await QuartzEndpoint.ScheduleRecurringSend(InputQueueAddress, new MyCancelableSchedule(scheduleId), new Interval {Name = "Joe"});

            await _done;

            var countBeforeCancel = _count;
            Assert.AreEqual(8, _count, "Expected to see 8 interval messages");

            await Bus.CancelScheduledRecurringSend(scheduledRecurringMessage);

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(10), new DoneAgain {Name = "Joe"});

            await _doneAgain;

            Assert.AreEqual(countBeforeCancel, _count, "Expected to see the count matches.");
        }

        [Test]
        [Explicit]
        public async Task Should_contain_additional_headers_that_provide_time_domain_context()
        {
            var scheduleId = Guid.NewGuid().ToString();

            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(10), new Done {Name = "Joe"});
            ScheduledRecurringMessage<Interval> scheduledRecurringMessage =
                await QuartzEndpoint.ScheduleRecurringSend(InputQueueAddress, new MySchedule(), new Interval {Name = "Joe"});

            await _done;

            Assert.Greater(_count, 0, "Expected to see at least one interval");


            Assert.IsNotNull(_lastInterval.Headers.Get<DateTimeOffset>(MessageHeaders.Quartz.Scheduled, null));
            Assert.IsNotNull(_lastInterval.Headers.Get<DateTimeOffset>(MessageHeaders.Quartz.Sent, null));
            Assert.IsNotNull(_lastInterval.Headers.Get<DateTimeOffset>(MessageHeaders.Quartz.NextScheduled, null));
            Assert.IsNotNull(_lastInterval.Headers.Get<DateTimeOffset>(MessageHeaders.Quartz.PreviousSent, null));

            Console.WriteLine("{0}", _lastInterval.Headers.Get<DateTimeOffset>(MessageHeaders.Quartz.NextScheduled, null));
        }

        [Test]
        [Explicit]
        public async Task Should_handle_now_properly()
        {
            await Scheduler.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(20), new Done {Name = "Joe"});
            await QuartzEndpoint.ScheduleRecurringSend(InputQueueAddress, new MySchedule(), new Interval {Name = "Joe"});


            await _done;

            Assert.AreEqual(8, _count, "Expected to see 8 interval messages");
        }

        Task<ConsumeContext<Done>> _done;
        Task<ConsumeContext<DoneAgain>> _doneAgain;
        int _count;
        ConsumeContext<Interval> _lastInterval;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;
            configurator.Handler<Interval>(async context =>
            {
                Interlocked.Increment(ref _count);
                _lastInterval = context;
            });

            _done = Handled<Done>(configurator);
            _doneAgain = Handled<DoneAgain>(configurator);
        }


        class MySchedule :
            DefaultRecurringSchedule
        {
            public MySchedule()
            {
                CronExpression = "0/1 * * * * ?";

                StartTime = DateTime.Now + TimeSpan.FromSeconds(3);
                EndTime = StartTime + TimeSpan.FromSeconds(7);

                Description = "my description";
            }
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
