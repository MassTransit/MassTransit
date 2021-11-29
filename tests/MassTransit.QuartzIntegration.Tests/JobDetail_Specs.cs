namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Quartz;
    using Quartz.Impl;


    [TestFixture]
    public class When_scheduling_a_job_using_quartz
    {
        [Test]
        public async Task Should_return_the_properties()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler().ConfigureAwait(false);
            await scheduler.Start().ConfigureAwait(false);

            var jobDetail = JobBuilder.Create<MyJob>()
                .UsingJobData("Body", "By Jake")
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(DateTime.UtcNow + TimeSpan.FromSeconds(1))
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger).ConfigureAwait(false);

            Assert.IsTrue(MyJob.Signaled.WaitOne(Utils.Timeout));

            Assert.AreEqual("By Jake", MyJob.SignaledBody);
        }

        [Test]
        public async Task Should_return_the_properties_with_custom_factory()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler().ConfigureAwait(false);
            await scheduler.Start().ConfigureAwait(false);

            var jobDetail = JobBuilder.Create<MyJob>()
                .UsingJobData("Body", "By Jake")
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(DateTime.UtcNow + TimeSpan.FromSeconds(1))
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger).ConfigureAwait(false);

            Assert.IsTrue(MyJob.Signaled.WaitOne(Utils.Timeout));

            Assert.AreEqual("By Jake", MyJob.SignaledBody);
        }


        class MyJob :
            IJob
        {
            static MyJob()
            {
                Signaled = new ManualResetEvent(false);
            }

            public static ManualResetEvent Signaled { get; private set; }
            public static string SignaledBody { get; private set; }

            public string Body { get; set; }

            public Task Execute(IJobExecutionContext context)
            {
                SignaledBody = Body;
                Signaled.Set();
                return Task.CompletedTask;
            }
        }
    }
}
