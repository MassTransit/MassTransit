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
    using NUnit.Framework;
    using Quartz;
    using Quartz.Impl;
    using TestFramework;


    public class QuartzInMemoryTestFixture :
        InMemoryTestFixture
    {
        IScheduler _scheduler;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();

            configurator.ReceiveEndpoint("quartz", x =>
            {
                x.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                x.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
            });
        }

        [TestFixtureSetUp]
        public void Setup_quartz_service()
        {
            _scheduler.JobFactory = new MassTransitJobFactory(Bus);
            _scheduler.Start();
        }

        [TestFixtureTearDown]
        public void Teardown_quartz_service()
        {
            if (_scheduler != null)
                _scheduler.Standby();
            if (_scheduler != null)
                _scheduler.Shutdown();
        }
    }
}