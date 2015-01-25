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
    using TestFramework;


    [TestFixture]
    public class Specifying_an_event_in_the_past :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            var endpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/quartz"));

            await endpoint.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromHours(-1), new A {Name = "Joe"});

            await handler;
        }


        class A
        {
            public string Name { get; set; }
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


//    [TestFixture, Explicit]
//    public class Sending_past_events_to_quartz
//    {
//        [Test]
//        public void Should_properly_send_the_message()
//        {
//            _bus.ScheduleMessage((-1).Minutes().FromUtcNow(), new A { Name = "Joe" }, x =>
//            {
//                x.SetHeader("TestHeader", "Test");
//            });
//
//            Assert.IsTrue(_receivedA.WaitOne(TimeSpan.FromMinutes(1)), "Message A not handled");
//
//            Assert.IsTrue(_received.Headers["TestHeader"].Equals("Test"));
//        }
//
//        [Test]
//        public void Should_not_handle_now()
//        {
//            _bus.ScheduleMessage(DateTime.UtcNow, new A { Name = "Joe" }, x =>
//            {
//                x.SetHeader("TestHeader", "Test");
//            });
//
//            Assert.IsTrue(_receivedA.WaitOne(Utils.Timeout), "Message A not handled");
//
//            Assert.IsTrue(_received.Headers["TestHeader"].Equals("Test"));
//        }
//
//        [Test]
//        public void Should_send_a_future_message()
//        {
//            _bus.ScheduleMessage((1).Seconds().FromUtcNow(), new A { Name = "Joe" }, x =>
//            {
//                x.SetHeader("TestHeader", "Test");
//            });
//
//            Assert.IsTrue(_receivedA.WaitOne(Utils.Timeout), "Message A not handled");
//
//            Assert.IsTrue(_received.Headers["TestHeader"].Equals("Test"));
//        }
//
//
//        class A 
//        {
//            public string Name { get; set; }
//        }
//
//        IServiceBus _bus;
//        ManualResetEvent _receivedA;
//        IConsumeContext<A> _received;
//
//        [TestFixtureSetUp]
//        public void Setup_quartz_service()
//        {
//            _receivedA = new ManualResetEvent(false);
//
//            _bus = ServiceBusFactory.New(x =>
//            {
//                x.ReceiveFrom("rabbitmq://localhost/test_quartz_client");
//                x.UseJsonSerializer();
//                x.UseRabbitMq();
//
//                x.Subscribe(s =>
//                {
//                    s.Handler<A>((msg, context) =>
//                    {
//                        _received = msg;
//                        _receivedA.Set();
//                    });
//                });
//            });
//
//        }
//
//        [TestFixtureTearDown]
//        public void Teardown_quartz_service()
//        {
//            if (_bus != null)
//                _bus.Dispose();
//        }
//    }
}