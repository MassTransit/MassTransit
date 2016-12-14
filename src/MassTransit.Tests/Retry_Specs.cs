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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class When_specifying_no_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;
            });
            await fault;

            _attempts.ShouldBe(1);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.None());

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
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;

                return TaskUtil.Completed;
            });
            await fault;

            _attempts.ShouldBe(1);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                _attempts++;
                throw new IntentionalTestException();
            });
        }
    }

    [TestFixture]
    public class When_specifying_retry_for_the_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_once()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;

                return TaskUtil.Completed;
            });
            await fault;

            Consumer.Attempts.ShouldBe(6);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new Consumer(), x =>
            {
                x.UseRetry(r => r.Immediate(5));
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public static int Attempts;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                Interlocked.Increment(ref Attempts);

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    public class When_specifying_the_bus_level_retry_policy :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_call_the_handler_twice()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), context =>
            {
                context.ResponseAddress = BusAddress;
                context.FaultAddress = BusAddress;
            });

            await fault;

            _attempts.ShouldBe(2);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureBus(configurator);
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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
        public async Task Should_only_call_the_inner_policy()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = SubscribeHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(x =>
            {
                x.ResponseAddress = BusAddress;
                x.FaultAddress = BusAddress;
            }));
            await fault;

            _attempts.ShouldBe(4);

            _lastAttempt.ShouldBe(3);
        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        int _attempts;
        int _lastAttempt;

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));

            base.ConfigureBus(configurator);
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(3));
            Handler<PingMessage>(configurator, async context =>
            {
                Interlocked.Increment(ref _attempts);

                _lastAttempt = context.GetRetryAttempt();

                throw new IntentionalTestException();
            });
        }
    }
}