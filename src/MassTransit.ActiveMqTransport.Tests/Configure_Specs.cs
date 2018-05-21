// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Using_the_handler_test_factory
    {
        ActiveMqTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new ActiveMqTestHarness();
            _handler = _harness.Handler<A>(async context =>
            {
                var endpoint = await context.GetSendEndpoint(context.SourceAddress);

                await endpoint.Send(new C());

                await context.Publish(new D());
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
            await _harness.Bus.Publish(new B());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_have_received_a_message_of_type_a()
        {
            Assert.That(_harness.Consumed.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_a()
        {
            Assert.That(_harness.Sent.Select<A>().Any(), Is.True);
        }

        [Test]
        public void Should_have_published_a_message_of_type_b()
        {
            Assert.That(_harness.Published.Select<B>().Any(), Is.True);
        }

        [Test]
        public void Should_have_sent_a_message_of_type_c()
        {
            Assert.That(_harness.Sent.Select<C>().Any(), Is.True);
        }

        [Test]
        public void Should_have_published_a_message_of_type_d()
        {
            Assert.That(_harness.Published.Select<D>().Any(), Is.True);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            Assert.That(_handler.Consumed.Select().Any(), Is.True);
        }


        class A
        {
        }


        class B
        {
        }


        class C
        {
        }


        class D
        {
        }
    }


    [TestFixture]
    public class Configuring_ActiveMQ
    {
        [Test]
        public async Task Should_succeed_and_connect_when_properly_configured()
        {
            TaskCompletionSource<bool> received = new TaskCompletionSource<bool>();

            Uri sendAddress = null;

            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                var host = cfg.Host("b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com", 61617, h =>
                {
                    h.Username("masstransit-build");
                    h.Password("build-Br0k3r");

                    h.UseSsl();
                });

                cfg.ReceiveEndpoint(host, "input-queue", x =>
                {
                    x.Handler<PingMessage>(async context =>
                    {
                        await context.Publish(new PongMessage(context.Message.CorrelationId));
                    });

                    sendAddress = x.InputAddress;
                });

                cfg.ReceiveEndpoint(host, "input-queue-too", x =>
                {
                    x.Handler<PongMessage>(async context =>
                    {
                        received.TrySetResult(true);
                    });
                });
            });

            await busControl.StartAsync();

            var sendEndpoint = await busControl.GetSendEndpoint(sendAddress);

            await sendEndpoint.Send(new PingMessage());

            await received.Task.UntilCompletedOrTimeout(TimeSpan.FromSeconds(5));

            await busControl.StopAsync();
        }

        [Test]
        public async Task Should_do_a_bunch_of_requests_and_responses()
        {
            var bus = Bus.Factory.CreateUsingActiveMq(sbc =>
            {
                var host = sbc.Host("b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com", 61617, h =>
                {
                    h.Username("masstransit-build");
                    h.Password("build-Br0k3r");

                    h.UseSsl();
                });

                sbc.ReceiveEndpoint(host, "test", e =>
                {
                    e.Handler<PingMessage>(async context => await context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
                });
            });

            await bus.StartAsync();
            try
            {
                for (var i = 0; i < 100; i = i + 1)
                {
                    var result = await bus.Request<PingMessage, PongMessage>(new PingMessage());
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }

        [Test]
        public async Task Should_connect_locally()
        {
            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host("localhost", 61616, h =>
                {
                    h.Username("admin");
                    h.Password("admin");
                });
            });

            await busControl.StartAsync(new CancellationTokenSource(10000).Token);

            await busControl.StopAsync(new CancellationTokenSource(10000).Token);
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness()
        {
            var harness = new ActiveMqTestHarness();

            await harness.Start();

            await harness.Stop();
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness_and_a_handler()
        {
            var harness = new ActiveMqTestHarness();
            var handler = harness.Handler<PingMessage>(async context =>
            {
            });

            await harness.Start();

            await harness.InputQueueSendEndpoint.Send(new PingMessage());

            await harness.Stop();
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness_and_a_publisher()
        {
            var harness = new ActiveMqTestHarness();
            var handler = harness.Handler<PingMessage>();
            var handler2 = harness.Handler<PongMessage>();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            Assert.That(handler.Consumed.Select().Any(), Is.True);

            //            await Task.Delay(20000);

            await harness.Bus.Publish(new PongMessage());

            Assert.That(handler2.Consumed.Select().Any(), Is.True);

            await harness.Stop().UntilCompletedOrTimeout(5000);

            await harness.Stop();
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness_and_publish_without_consumer()
        {
            var harness = new ActiveMqTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            await harness.Stop();
        }

        [Test]
        public void Should_succeed_when_properly_configured()
        {
            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host("b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com", 61617, h =>
                {
                    h.Username("masstransit-build");
                    h.Password("build-Br0k3r");

                    h.UseSsl();
                });
            });
        }
    }
}