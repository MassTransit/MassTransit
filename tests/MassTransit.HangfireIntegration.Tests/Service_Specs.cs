// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using Scheduling;


    [TestFixture]
    public class Using_the_hangfire_service_with_json :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Bus.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

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
    public class Using_the_hangfire_service_with_xml :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();
            Task<ConsumeContext<IA>> handlerIA = SubscribeHandler<IA>();

            await Bus.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(1), new A {Name = "Joe"});

            await handlerA;

            ConsumeContext<IA> context = await handlerIA;
            //Assert.IsTrue(context.GetQuartzSent().HasValue);
        }


        class A : IA
        {
            public string Name { get; set; }
        }


        class IA
        {
            string Id { get; set; }
        }


        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();

            base.ConfigureInMemoryBus(configurator);
        }
    }


    [TestFixture]
    public class Using_the_hangfire_service_and_cancelling :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handlerA = SubscribeHandler<A>();

            ScheduledMessage<A> scheduledMessage =
                await Bus.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(3), new A {Name = "Joe"});

            await Task.Delay(1000);

            await Bus.CancelScheduledSend(scheduledMessage);

            Assert.That(async () => await handlerA.OrTimeout(5000), Throws.TypeOf<TimeoutException>());
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
}
