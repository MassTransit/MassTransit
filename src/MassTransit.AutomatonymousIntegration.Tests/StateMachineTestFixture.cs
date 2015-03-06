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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using NUnit.Framework;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using TestFramework;


    public class StateMachineTestFixture :
        InMemoryTestFixture
    {
        Uri _quartzQueueAddress;
        ISendEndpoint _quartzQueueSendEndpoint;

        public StateMachineTestFixture()
        {
            _quartzQueueAddress = new Uri("loopback://localhost/quartz");
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint QuartzQueueSendEndpoint
        {
            get { return _quartzQueueSendEndpoint; }
        }

        protected Uri QuartzQueueAddress
        {
            get { return _quartzQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _quartzQueueAddress = value;
            }
        }

        IScheduler _scheduler;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();

            configurator.ReceiveEndpoint("quartz", x =>
            {
                x.Consumer(() => new ScheduleMessageConsumer(_scheduler));
            });
        }

        [TestFixtureSetUp]
        public void Setup_quartz_service()
        {
            _scheduler.JobFactory = new MassTransitJobFactory(Bus);
            _scheduler.Start();

            _quartzQueueSendEndpoint = GetSendEndpoint(_quartzQueueAddress).Result;
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
