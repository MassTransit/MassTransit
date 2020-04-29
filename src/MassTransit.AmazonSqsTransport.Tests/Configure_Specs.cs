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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Configuration;
    using Context;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Logging;
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

        [Test]
        public async Task Should_support_a_simple_handler_on_publish()
        {
            await _harness.Bus.Publish(new A());

            var publishedMessageId = _harness.Published.Select<A>().First().Context.MessageId;
            Assert.That(_handler.Consumed.Select(c => c.Context.MessageId == publishedMessageId).Any(), Is.True);
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
        static int _subscribedObserver;

        [OneTimeSetUp]
        public void Setup()
        {
            var loggerFactory = new TestOutputLoggerFactory(true);

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            if (Interlocked.CompareExchange(ref _subscribedObserver, 1, 0) == 0)
                DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());
        }

        [Test]
        public async Task Should_succeed_and_connect_when_properly_configured()
        {
            TaskCompletionSource<bool> received = new TaskCompletionSource<bool>();

            IAmazonSqsHost host = null;

            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                host = cfg.Host("us-east-2", h =>
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
            try
            {
                await busControl.Publish(new PingMessage());

                await received.Task.OrTimeout(TimeSpan.FromSeconds(30));
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        [Test]
        public async Task Should_do_a_bunch_of_requests_and_responses()
        {
            var bus = Bus.Factory.CreateUsingAmazonSqs(sbc =>
            {
                var host = sbc.Host("us-east-2", h =>
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
                cfg.Host(new Uri("amazonsqs://localhost:4576"), h =>
                {
                    h.AccessKey("admin");
                    h.SecretKey("admin");
                    h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://localhost:4575" });
                    h.Config(new AmazonSQSConfig { ServiceURL = "http://localhost:4576" });
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

            await harness.Stop().OrTimeout(s:5);

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
        public async Task Should_connect_with_accessKey_and_secretKey()
        {
            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host("ap-southeast-2", h =>
                {
                    h.AccessKey(AwsAccessKey);
                    h.SecretKey(AwsSecretKey);
                });
            });

            try
            {
                await busControl.StartAsync();
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        [Test]
        public async Task Should_connect_with_credentials()
        {
            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host("ap-southeast-2", h =>
                {
                    var credentials = new BasicAWSCredentials(AwsAccessKey, AwsSecretKey);
                    h.Credentials(credentials);
                });
            });

            try
            {
                await busControl.StartAsync();
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        [Test]
        public async Task Should_create_queue_with_multiple_subscriptions()
        {
            var messageTypes = new[]
            {
                typeof(Message0), typeof(Message1), typeof(Message2), typeof(Message3), typeof(Message4), typeof(Message5), typeof(Message6),
                typeof(Message7), typeof(Message8), typeof(Message9), typeof(Message10), typeof(Message11), typeof(Message12), typeof(Message13),
                typeof(Message14), typeof(Message15), typeof(Message16), typeof(Message17), typeof(Message18), typeof(Message19), typeof(Message20),
                typeof(Message21), typeof(Message22)
            };

            var tasksCompleted = messageTypes.ToDictionary(k => k, v => new TaskCompletionSource<bool>());

            IAmazonSqsHost host = null;

            var busControl = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                host = cfg.Host("ap-southeast-2", h =>
                {
                    h.AccessKey(AwsAccessKey);
                    h.SecretKey(AwsSecretKey);
                });

                Func<object, Task> receiveTask = t =>
                {
                    tasksCompleted[t.GetType()].SetResult(true);
                    return Util.TaskUtil.Completed;
                };

                cfg.ReceiveEndpoint(host, "long_multi_subs_queue", e =>
                {
                    e.Handler<Message0>(async c => await receiveTask(c.Message));
                    e.Handler<Message1>(async c => await receiveTask(c.Message));
                    e.Handler<Message2>(async c => await receiveTask(c.Message));
                    e.Handler<Message3>(async c => await receiveTask(c.Message));
                    e.Handler<Message4>(async c => await receiveTask(c.Message));
                    e.Handler<Message5>(async c => await receiveTask(c.Message));
                    e.Handler<Message6>(async c => await receiveTask(c.Message));
                    e.Handler<Message7>(async c => await receiveTask(c.Message));
                    e.Handler<Message8>(async c => await receiveTask(c.Message));
                    e.Handler<Message9>(async c => await receiveTask(c.Message));
                    e.Handler<Message10>(async c => await receiveTask(c.Message));
                    e.Handler<Message11>(async c => await receiveTask(c.Message));
                    e.Handler<Message12>(async c => await receiveTask(c.Message));
                    e.Handler<Message13>(async c => await receiveTask(c.Message));
                    e.Handler<Message14>(async c => await receiveTask(c.Message));
                    e.Handler<Message15>(async c => await receiveTask(c.Message));
                    e.Handler<Message16>(async c => await receiveTask(c.Message));
                    e.Handler<Message17>(async c => await receiveTask(c.Message));
                    e.Handler<Message18>(async c => await receiveTask(c.Message));
                    e.Handler<Message19>(async c => await receiveTask(c.Message));
                    e.Handler<Message20>(async c => await receiveTask(c.Message));
                    e.Handler<Message21>(async c => await receiveTask(c.Message));
                    e.Handler<Message22>(async c => await receiveTask(c.Message));
                });
            });

            await busControl.StartAsync();

            var publishTasks = messageTypes.Select(m => busControl.Publish(Activator.CreateInstance(m)));
            await Task.WhenAll(publishTasks);

            var awaitTasks = tasksCompleted.Values.Select(async t => await t.Task.OrTimeout(TimeSpan.FromSeconds(20)));
            await Task.WhenAll(awaitTasks);

            await busControl.StopAsync();
        }

        public class Message0 { }
        public class Message1 { }
        public class Message2 { }
        public class Message3 { }
        public class Message4 { }
        public class Message5 { }
        public class Message6 { }
        public class Message7 { }
        public class Message8 { }
        public class Message9 { }
        public class Message10 { }
        public class Message11 { }
        public class Message12 { }
        public class Message13 { }
        public class Message14 { }
        public class Message15 { }
        public class Message16 { }
        public class Message17 { }
        public class Message18 { }
        public class Message19 { }
        public class Message20 { }
        public class Message21 { }
        public class Message22 { }
    }
}
