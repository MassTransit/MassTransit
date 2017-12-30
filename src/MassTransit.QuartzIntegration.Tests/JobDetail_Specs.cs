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
namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Quartz;
    using Quartz.Impl;
    using Util;


    [TestFixture]
    public class When_scheduling_a_job_using_quartz
    {
        [Test]
        public async Task Should_return_the_properties()
        {
            var factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler().ConfigureAwait(false);
            await scheduler.Start().ConfigureAwait(false);

            IJobDetail jobDetail = JobBuilder.Create<MyJob>()
                .UsingJobData("Body", "By Jake")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
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
            IScheduler scheduler = await factory.GetScheduler().ConfigureAwait(false);
            scheduler.JobFactory = new MassTransitJobFactory(null);
            await scheduler.Start().ConfigureAwait(false);;

            IJobDetail jobDetail = JobBuilder.Create<MyJob>()
                .UsingJobData("Body", "By Jake")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
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
                return TaskUtil.Completed;
            }
        }
    }
}