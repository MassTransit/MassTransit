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
    using Scheduling;


    [TestFixture]
    public class Specifying_a_recurring_event :
        QuartzInMemoryTestFixture
    {
        [Test, Explicit]
        public async void Should_handle_now_properly()
        {
            await QuartzEndpoint.ScheduleSend(InputQueueAddress, DateTime.UtcNow + TimeSpan.FromSeconds(20), new Done {Name = "Joe"});
            await QuartzEndpoint.ScheduleRecurringSend(InputQueueAddress, new MySchedule(), new Interval {Name = "Joe"});


            await _done;

            Assert.AreEqual(8, _count, "Expected to see 8 interval messages");
        }

        Task<ConsumeContext<Done>> _done;
        int _count;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;
            configurator.Handler<Interval>(async context =>
            {
                Interlocked.Increment(ref _count);
            });

            _done = Handled<Done>(configurator);
        }


        class MySchedule :
            DefaultRecurringSchedule
        {
            public MySchedule()
            {
                CronExpression = "0/1 * * * * ?";

                StartTime = DateTime.Now + TimeSpan.FromSeconds(3);
                EndTime = StartTime + TimeSpan.FromSeconds(7);
            }
        }


        public class Interval
        {
            public string Name { get; set; }
        }


        public class Done
        {
            public string Name { get; set; }
        }
    }
}