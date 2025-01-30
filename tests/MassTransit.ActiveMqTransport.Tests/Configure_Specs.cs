namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Internals;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;
    using Topology;
    using Util;


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    public class Using_the_handler_test_factory
    {
        public Using_the_handler_test_factory(string protocol)
        {
            this.protocol = protocol;
        }

        [Test]
        public void Should_have_published_a_message_of_type_b()
        {
            Assert.That(_harness.Published.Select<B>().Any(), Is.True);
        }

        [Test]
        public void Should_have_published_a_message_of_type_d()
        {
            Assert.That(_harness.Published.Select<D>().Any(), Is.True);
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
        public void Should_have_sent_a_message_of_type_c()
        {
            Assert.That(_harness.Sent.Select<C>().Any(), Is.True);
        }

        [Test]
        public void Should_support_a_simple_handler()
        {
            Assert.That(_handler.Consumed.Select().Any(), Is.True);
        }

        ActiveMqTestHarness _harness;
        HandlerTestHarness<A> _handler;
        private readonly string protocol;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new ActiveMqTestHarness(protocol);
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
        public void Failover_should_take_precendence_in_uri_construction_activemq()
        {
            var settings = new OpenWireHostSettings(new Uri($"{ActiveMqHostAddress.ActiveMqScheme}://fake-host"))
            {
                Port = 61616,
                FailoverHosts = new[] { "failover1", "failover2" },
            };
            settings.TransportOptions.Add("reconnectAttempts", "-1");

            Assert.That(settings.BrokerAddress,
                Is.EqualTo(new Uri("activemq:failover:(tcp://failover1:61616/?wireFormat.tightEncodingEnabled=true,tcp://failover2:61616/?wireFormat.tightEncodingEnabled=true)?transport.reconnectAttempts=-1")));
        }

        [Test]
        public void Failover_should_take_precendence_in_uri_construction_amqp()
        {
            var settings = new AmqpHostSettings(new Uri($"{ActiveMqHostAddress.AmqpScheme}://fake-host"))
            {
                Port = 61616,
                FailoverHosts = new[] { "failover1", "failover2" },
            };
            settings.TransportOptions.Add("reconnectAttempts", "-1");

            Assert.That(settings.BrokerAddress,
                Is.EqualTo(new Uri("failover:(amqp://failover1:61616/?,amqp://failover2:61616/?)?transport.reconnectAttempts=-1")));
        }

        [Test]
        public async Task Pub_Sub_Queue_Names_Should_Not_Contain_Periods()
        {
            var consumeTopology = new ActiveMqConsumeTopology(null);
            var queueName = consumeTopology.CreateTemporaryQueueName("bus.test");
            Assert.That(queueName, Does.Not.Contain('.'));
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally(string protocol)
        {
            var host = (protocol == ActiveMqHostAddress.AmqpScheme)
                ? new Uri("amqp://localhost:5672")
                : new Uri("activemq://localhost:61616");
            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host(host, h =>
                {
                    h.Username("admin");
                    h.Password("admin");
                });
            });

            await busControl.StartAsync(new CancellationTokenSource(10000).Token);

            await busControl.StopAsync(new CancellationTokenSource(10000).Token);
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally_with_test_harness(string protocol)
        {
            var harness = new ActiveMqTestHarness(protocol);

            await harness.Start();

            await harness.Stop();
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally_with_test_harness_and_a_handler(string protocol)
        {
            var harness = new ActiveMqTestHarness(protocol);
            HandlerTestHarness<PingMessage> handler = harness.Handler<PingMessage>(async context =>
            {
            });

            await harness.Start();

            await harness.InputQueueSendEndpoint.Send(new PingMessage());

            await harness.Stop();
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally_with_test_harness_and_a_publisher(string protocol)
        {
            var harness = new ActiveMqTestHarness(protocol);
            HandlerTestHarness<PingMessage> handler = harness.Handler<PingMessage>();
            HandlerTestHarness<PongMessage> handler2 = harness.Handler<PongMessage>();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            Assert.That(handler.Consumed.Select().Any(), Is.True);

            //            await Task.Delay(20000);

            await harness.Bus.Publish(new PongMessage());

            Assert.That(handler2.Consumed.Select().Any(), Is.True);

            await harness.Stop().OrTimeout(s: 5);

            await harness.Stop();
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally_with_test_harness_and_a_publisher_happy(string protocol)
        {
            var harness = new ActiveMqTestHarness(protocol);
            HandlerTestHarness<PingMessage> handler = harness.Handler<PingMessage>();

            await harness.Start();

            for (var i = 0; i < 100; i++)
                await harness.Bus.Publish(new PingMessage());

            var count = await handler.Consumed.SelectAsync().Count();

            Assert.That(count, Is.EqualTo(100));

            await harness.Stop().OrTimeout(s: 5);
        }

        [Test]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        public async Task Should_connect_locally_with_test_harness_and_publish_without_consumer(string protocol)
        {
            var harness = new ActiveMqTestHarness(protocol);

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            await harness.Stop();
        }

        [Test]
        [Category("Flaky")]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        [TestCase("artemis")]
        public async Task Should_do_a_bunch_of_requests_and_responses(string flavor)
        {
            var bus = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.ConfigureHost(flavor);
                if (flavor == "artemis")
                {
                    cfg.SetTemporaryQueueNamePrefix("myprefix.");
                }


                cfg.ReceiveEndpoint("test", e =>
                {
                    e.Handler<PingMessage>(async context => await context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
                });
            });

            await bus.StartAsync();
            try
            {
                for (var i = 0; i < 100; i += 1)
                {
                    Response<PongMessage> result = await bus.Request<PingMessage, PongMessage>(new PingMessage());
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }

        [Test]
        [Category("Flaky")]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        [TestCase("artemis")]
        public async Task Should_do_a_bunch_of_requests_and_responses_explicit_configuration(string flavor)
        {
            var bus = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.ConfigureHost(flavor);
                if (flavor == "artemis")
                {
                    cfg.SetConsumerEndpointQueueNameFormatter(new ArtemisConsumerEndpointQueueNameFormatter());
                    cfg.SetTemporaryQueueNameFormatter(new PrefixTemporaryQueueNameFormatter("myprefix."));
                }

                cfg.ReceiveEndpoint("test", e =>
                {
                    e.Handler<PingMessage>(async context => await context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
                });
            });

            await bus.StartAsync();
            try
            {
                for (var i = 0; i < 100; i += 1)
                {
                    Response<PongMessage> result = await bus.Request<PingMessage, PongMessage>(new PingMessage());
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }

        [Test]
        [Category("Flaky")]
        [TestCase(ActiveMqHostAddress.ActiveMqScheme)]
        [TestCase(ActiveMqHostAddress.AmqpScheme)]
        [TestCase("artemis")]
        public async Task Should_succeed_and_connect_when_properly_configured(string flavor)
        {
            TaskCompletionSource<bool> received = TaskUtil.GetTask<bool>();

            Uri sendAddress = null;

            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.ConfigureHost(flavor);

                cfg.ReceiveEndpoint("input-queue", x =>
                {
                    x.Handler<PingMessage>(async context =>
                    {
                        await context.Publish(new PongMessage(context.Message.CorrelationId));
                    });

                    sendAddress = x.InputAddress;
                });

                cfg.ReceiveEndpoint("input-queue-too", x =>
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
    }
}
