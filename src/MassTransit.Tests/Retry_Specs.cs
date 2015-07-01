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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Monitoring.Introspection.Contracts;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_specifying_no_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));
            await fault;

            _attempts.ShouldBe(1);
        }

        [Test]
        public async void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = await Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(Retry.None);

            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_specifying_the_default_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));
            await fault;

            _attempts.ShouldBe(1);
        }

        [Test]
        public async void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = await Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_specifying_the_bus_level_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_only_call_the_handler_twice()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));

            await fault;

            _attempts.ShouldBe(2);
        }

        [Test]
        public async void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = await Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(Retry.Immediate(1));

            base.ConfigureBus(configurator);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }


    [TestFixture]
    public class When_both_levels_of_retry_are_specified :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_only_call_the_inner_policy()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));
            await fault;

            _attempts.ShouldBe(4);
        }

        [Test]
        public async void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = await Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(Retry.Immediate(1));

            base.ConfigureBus(configurator);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(Retry.Immediate(3));
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }
}