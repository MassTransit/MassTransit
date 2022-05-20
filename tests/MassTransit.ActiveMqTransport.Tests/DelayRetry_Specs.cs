namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Using_the_delayed_exchange :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(context =>
            {
                if (_timer == null)
                    _timer = Stopwatch.StartNew();

                if (_count++ < 2)
                {
                    Console.WriteLine("{0} now is not a good time", DateTime.UtcNow);
                    throw new IntentionalTestException("I'm so not ready for this jelly.");
                }

                _timer.Stop();

                Console.WriteLine("{0} okay, now is good (retried {1} times)", DateTime.UtcNow, context.Headers.Get("MT-Redelivery-Count", default(int?)));

                // okay, ready.
                _receivedTimeSpan = _timer.Elapsed;
                _received.TrySetResult(context);

                return Task.CompletedTask;
            }, x => x.UseDelayedRedelivery(r => r.Intervals(1000, 2000)));
        }
    }


    [TestFixture]
    public class Delaying_a_message_retry_with_policy :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_only_defer_up_to_the_retry_count()
        {
            var pingMessage = new PingMessage();

            Task<ConsumeContext<Fault<PingMessage>>> fault =
                SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(3));
        }

        int _count;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            configurator.Handler<PingMessage>(context =>
            {
                Interlocked.Increment(ref _count);

                throw new IntentionalTestException();
            }, x => x.UseDelayedRedelivery(r => r.Intervals(100, 200)));
        }
    }


    [TestFixture]
    public class Retrying_a_message_retry_with_policy :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_only_retry_up_to_the_retry_count()
        {
            var pingMessage = new PingMessage();

            Task<ConsumeContext<Fault<PingMessage>>> fault =
                SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(3));
        }

        int _count;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            configurator.Handler<PingMessage>(context =>
            {
                Interlocked.Increment(ref _count);

                throw new IntentionalTestException();
            }, x => x.UseRetry(r => r.Intervals(100, 200)));
        }
    }


    [TestFixture]
    public class Using_delayed_exchange_redelivery_with_a_consumer :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_retry_each_message_type()
        {
            var pingMessage = new OneMessage { CorrelationId = NewId.NextGuid() };

            Task<ConsumeContext<Fault<OneMessage>>> pingFault =
                SubscribeHandler<Fault<OneMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);
            Task<ConsumeContext<Fault<TwoMessage>>> pongFault =
                SubscribeHandler<Fault<TwoMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);
            await InputQueueSendEndpoint.Send(new TwoMessage { CorrelationId = pingMessage.CorrelationId }, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<OneMessage>> pingFaultContext = await pingFault;
            ConsumeContext<Fault<TwoMessage>> pongFaultContext = await pongFault;

            Assert.That(_consumer.PingCount, Is.EqualTo(3));
            Assert.That(_consumer.PongCount, Is.EqualTo(3));
        }

        Consumer _consumer;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.UseDelayedRedelivery(r => r.Intervals(100, 200));

            _consumer = new Consumer();
            configurator.Consumer(() => _consumer);
        }


        class Consumer :
            IConsumer<OneMessage>,
            IConsumer<TwoMessage>
        {
            public int PingCount;
            public int PongCount;

            public Task Consume(ConsumeContext<OneMessage> context)
            {
                Interlocked.Increment(ref PingCount);

                throw new IntentionalTestException();
            }

            public Task Consume(ConsumeContext<TwoMessage> context)
            {
                Interlocked.Increment(ref PongCount);

                throw new IntentionalTestException();
            }
        }


        public class OneMessage
        {
            public Guid CorrelationId { get; set; }
        }


        public class TwoMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Delayed_redelivery
    {
        Consumer _consumer;

        [Test]
        [Category("Flaky")]
        [TestCase("activemq")]
        [TestCase("artemis")]
        public async Task Should_properly_redeliver(string flavor)
        {
            TaskCompletionSource<bool> received = TaskUtil.GetTask<bool>();

            Uri sendAddress = null;

            var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                BusTestFixture.ConfigureBusDiagnostics(cfg);

                if (flavor == "artemis")
                {
                    cfg.Host("localhost", 61618, cfgHost =>
                    {
                        cfgHost.Username("admin");
                        cfgHost.Password("admin");
                    });
                    cfg.EnableArtemisCompatibility();
                }

                cfg.ReceiveEndpoint("input-queue", x =>
                {
                    x.ConfigureConsumeTopology = false;

                    x.UseDelayedRedelivery(r => r.Intervals(2000, 5000));

                    _consumer = new Consumer();
                    x.Consumer(() => _consumer);
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

            var pingMessage = new OneMessage { CorrelationId = NewId.NextGuid() };

            Task<ConsumeContext<Fault<OneMessage>>> pingFault =
                SubscribeHandler<Fault<OneMessage>>(busControl, x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);
            Task<ConsumeContext<Fault<TwoMessage>>> pongFault =
                SubscribeHandler<Fault<TwoMessage>>(busControl, x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await sendEndpoint.Send(pingMessage, x => x.FaultAddress = busControl.Address);
            await sendEndpoint.Send(new TwoMessage { CorrelationId = pingMessage.CorrelationId }, x => x.FaultAddress = busControl.Address);

            ConsumeContext<Fault<OneMessage>> pingFaultContext = await pingFault;
            ConsumeContext<Fault<TwoMessage>> pongFaultContext = await pongFault;

            Assert.That(_consumer.PingCount, Is.EqualTo(3));
            Assert.That(_consumer.PongCount, Is.EqualTo(3));

            await busControl.StopAsync();
        }

        public Task<ConsumeContext<T>> SubscribeHandler<T>(IBus bus, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            TaskCompletionSource<ConsumeContext<T>> source = TaskUtil.GetTask<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = bus.ConnectHandler<T>(async context =>
            {
                if (filter(context))
                {
                    handler.Disconnect();

                    source.SetResult(context);
                }
            });

            return source.Task;
        }


        class Consumer :
            IConsumer<OneMessage>,
            IConsumer<TwoMessage>
        {
            public int PingCount;
            public int PongCount;

            public Task Consume(ConsumeContext<OneMessage> context)
            {
                Interlocked.Increment(ref PingCount);

                throw new IntentionalTestException();
            }

            public Task Consume(ConsumeContext<TwoMessage> context)
            {
                Interlocked.Increment(ref PongCount);

                throw new IntentionalTestException();
            }
        }


        public class OneMessage
        {
            public Guid CorrelationId { get; set; }
        }


        public class TwoMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Using_delayed_exchange_redelivery_with_a_consumer_and_retry :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_retry_and_redeliver()
        {
            var pingMessage = new PingMessage();

            Task<ConsumeContext<Fault<PingMessage>>> pingFault =
                SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);
            await InputQueueSendEndpoint.Send(new PongMessage(pingMessage.CorrelationId), x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> pingFaultContext = await pingFault;

            Assert.That(Consumer.PingCount, Is.EqualTo(6));
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.UseDelayedRedelivery(r => r.Intervals(100));
            configurator.UseMessageRetry(x => x.Immediate(2));

            Consumer.PingCount = 0;

            configurator.Consumer(() => new Consumer());
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public static int PingCount;

            public Consumer()
            {
                LogContext.Info?.Log("Creating consumer");
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                Interlocked.Increment(ref PingCount);

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    public class Delaying_a_message_retry_with_policy_but_no_retries :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_immediately_fault_with_no_delay()
        {
            var pingMessage = new PingMessage();

            Task<ConsumeContext<Fault<PingMessage>>> fault =
                SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(1));
        }

        int _count;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            configurator.Handler<PingMessage>(context =>
            {
                Interlocked.Increment(ref _count);

                throw new IntentionalTestException();
            }, x => x.UseDelayedRedelivery(r => r.None()));
        }
    }


    [TestFixture]
    public class Explicitly_deferring_a_message_instead_of_throwing :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_timer == null)
                    _timer = Stopwatch.StartNew();

                if (_count++ < 2)
                {
                    Console.WriteLine("{0} now is not a good time", DateTime.UtcNow);

                    await context.Defer(TimeSpan.FromMilliseconds(1000));
                    return;
                }

                _timer.Stop();

                Console.WriteLine("{0} okay, now is good (retried {1} times)", DateTime.UtcNow, context.Headers.Get("MT-Redelivery-Count", default(int?)));

                // okay, ready.
                _receivedTimeSpan = _timer.Elapsed;
                _received.TrySetResult(context);
            });
        }
    }


    [TestFixture]
    public class execute_callback_function_during_defer :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_execute_callback_during_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
            Assert.IsTrue(_hit);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;
        bool _hit;

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_timer == null)
                    _timer = Stopwatch.StartNew();

                if (_count++ < 2)
                {
                    Console.WriteLine("{0} now is not a good time", DateTime.UtcNow);

                    await context.Defer(TimeSpan.FromMilliseconds(1000), (consumeContext, sendContext) =>
                    {
                        _hit = true;
                    });

                    return;
                }

                _timer.Stop();

                Console.WriteLine("{0} okay, now is good (retried {1} times)", DateTime.UtcNow, context.Headers.Get("MT-Redelivery-Count", default(int?)));

                // okay, ready.
                _receivedTimeSpan = _timer.Elapsed;
                _received.TrySetResult(context);
            });
        }
    }
}
