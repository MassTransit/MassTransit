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
namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Configuration;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Using_the_handler_test_factory
    {
        AmazonSqsTestHarness _harness;
        HandlerTestHarness<A> _handler;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new AmazonSqsTestHarness();
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
    public class Configuring_AmazonSQS
    {
        const string AwsAccessKey = "{YOUR AWS ACCESS KEY}";
        const string AwsSecretKey = "{YOUR AWS SECRET KEY}";

        [Test]
        public async Task Should_succeed_and_connect_when_properly_configured()
        {
            TaskCompletionSource<bool> received = new TaskCompletionSource<bool>();

            IAmazonSqsHost host = null;

            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                host = cfg.Host("ap-southeast-2", h =>
                {
                    h.AccessKey(AwsAccessKey);
                    h.SecretKey(AwsSecretKey);
                });

                cfg.ReceiveEndpoint(host, "input-queue", x =>
                {
                    x.Handler<PingMessage>(async context =>
                    {
                        await context.Publish(new PongMessage(context.Message.CorrelationId));
                    });
                });

                cfg.ReceiveEndpoint(host, "input-queue-too", x =>
                {
                    x.Handler<PongMessage>(async context =>
                    {
                        received.TrySetResult(true);

                        await Util.TaskUtil.Completed;
                    });
                });
            });

            await busControl.StartAsync();

            var sendAddress = host.Topology.GetDestinationAddress(typeof(PingMessage));

            var sendEndpoint = await busControl.GetSendEndpoint(sendAddress);

            await sendEndpoint.Send(new PingMessage());

            await received.Task.UntilCompletedOrTimeout(TimeSpan.FromSeconds(120));

            await busControl.StopAsync();
        }

        [Test]
        public async Task Should_do_a_bunch_of_requests_and_responses()
        {
            var bus = Bus.Factory.CreateUsingAmazonSqs(sbc =>
            {
                var host = sbc.Host("ap-southeast-2", h =>
                {
                    h.AccessKey(AwsAccessKey);
                    h.SecretKey(AwsSecretKey);
                });

                sbc.ReceiveEndpoint(host, "test", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                    });
                });
            });

            await bus.StartAsync();
            try
            {
                for (var i = 0; i < 100; i = i + 1)
                {
                    var result = await bus.Request<PingMessage, PongMessage>(new PingMessage(), timeout: TimeSpan.FromSeconds(60));
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
            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host(new Uri("amazonsqs://docker.localhost:4576"), h =>
                {
                    h.AccessKey("admin");
                    h.SecretKey("admin");
                    h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://docker.localhost:4575" });
                    h.Config(new AmazonSQSConfig { ServiceURL = "http://docker.localhost:4576" });
                });
            });

            await busControl.StartAsync(new CancellationTokenSource(10000).Token);

            await busControl.StopAsync(new CancellationTokenSource(10000).Token);
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness()
        {
            var harness = new AmazonSqsTestHarness();

            await harness.Start();

            await harness.Stop();
        }

        [Test]
        public async Task Should_connect_locally_with_test_harness_and_a_handler()
        {
            var harness = new AmazonSqsTestHarness();
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
            var harness = new AmazonSqsTestHarness();
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
            var harness = new AmazonSqsTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            await harness.Stop();
        }

        [Test]
        public void Should_succeed_when_properly_configured()
        {
            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host("ap-southeast-2", h =>
                {
                    h.AccessKey(AwsAccessKey);
                    h.SecretKey(AwsSecretKey);
                });
            });
        }
    }
}
