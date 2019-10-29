namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configurators;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;
    using Topology.Topologies;
    using Util;


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
        const string TestBrokerHost = "b-15a8b984-a883-4143-a4e7-8f97bc5db37d-1.mq.us-east-2.amazonaws.com";
        const string TestUsername = "masstransit-build";
        const string TestPassword = "build-Br0k3r";

        readonly string[] FailoverHosts = new string[]
        {

        };


        [Test]
        public async Task Should_succeed_and_connect_when_properly_configured()
        {
            TaskCompletionSource<bool> received = TaskUtil.GetTask<bool>();

            Uri sendAddress = null;

            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                var host = cfg.Host(TestBrokerHost, 61617, h =>
                {
                    h.Username(TestUsername);
                    h.Password(TestPassword);

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

            await received.Task.OrTimeout(TimeSpan.FromSeconds(5));

            await busControl.StopAsync();
        }

        [Test]
        public async Task Should_do_a_bunch_of_requests_and_responses()
        {
            var bus = Bus.Factory.CreateUsingActiveMq(sbc =>
            {
                var host = sbc.Host(TestBrokerHost, 61617, h =>
                {
                    h.Username(TestUsername);
                    h.Password(TestPassword);

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
        public async Task Pub_Sub_Queue_Names_Should_Not_Contain_Periods()
        {
            var consumeTopology = new ActiveMqConsumeTopology(null, null);
            var queueName = consumeTopology.CreateTemporaryQueueName("bus.test");
            Assert.That(queueName, Does.Not.Contain('.'));
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

            await harness.Stop().OrTimeout(s:5);

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
                cfg.Host(TestBrokerHost, 61617, h =>
                {
                    h.Username(TestUsername);
                    h.Password(TestPassword);

                    h.UseSsl();
                });
            });
        }

        [Test]
        public void Failover_should_take_precendence_in_uri_construction()
        {
            var settings = new ConfigurationHostSettings()
            {
                Host = "fake-host",
                Port = 61616,
                FailoverHosts = new []
                {
                    "failover1",
                    "failover2"
                }
            };

            Assert.That(settings.BrokerAddress, Is.EqualTo(new Uri("activemq:failover:(tcp://failover1:61616/,tcp://failover2:61616/)")));
        }

        [Test]
        public async Task Should_do_a_bunch_of_requests_and_responses_on_failover_transport()
        {
            if (FailoverHosts.Length == 0)
            {
                // Ignoring this test if there are no failovers
                return;
            }

            var bus = Bus.Factory.CreateUsingActiveMq(sbc =>
            {
                var host = sbc.Host("activemq-cluster", 61617, h =>
                {
                    h.Username(TestUsername);
                    h.Password(TestPassword);
                    h.FailoverHosts(FailoverHosts);
                    h.TransportOptions(new Dictionary<string, string>()
                    {
                        { "transport.randomize", "true" }
                    });

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
    }
}
