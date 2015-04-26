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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Quartz;
    using Quartz.Impl;
    using Scheduling;
    using TestFramework;


    [TestFixture]
    public class Using_the_quartz_service_with_json :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async void Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Bus.ScheduleMessage(DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

            await handlerA;
            await handlerIA;
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }
    }


    [TestFixture]
    public class Using_the_quartz_service_with_xml :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async void Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Bus.ScheduleMessage(DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

            await handlerA;
            await handlerIA;
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }


        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();

            base.ConfigureBus(configurator);
        }
    }


    [TestFixture]
    public class Using_the_quartz_service_and_cancelling :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            ScheduledMessage<A> scheduledMessage =
                await Bus.ScheduleMessage(DateTime.UtcNow + TimeSpan.FromSeconds(8), new A {Name = "Joe"});


            await Bus.CancelScheduledMessage(scheduledMessage);


            await handlerA;
            await handlerIA;
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }


        IScheduler _scheduler;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseJsonSerializer();

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