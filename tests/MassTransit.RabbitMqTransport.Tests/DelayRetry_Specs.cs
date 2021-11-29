namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    [Category("Flaky")]
    public class Using_the_delayed_exchange :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
    [Category("Flaky")]
    public class Delaying_a_message_retry_with_policy :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
    [Category("Flaky")]
    public class Retrying_a_message_retry_with_policy :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
    [Category("Flaky")]
    public class Using_delayed_exchange_redelivery_with_a_consumer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_retry_each_message_type()
        {
            var pingMessage = new PingMessage();

            Task<ConsumeContext<Fault<PingMessage>>> pingFault =
                SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);
            Task<ConsumeContext<Fault<PongMessage>>> pongFault =
                SubscribeHandler<Fault<PongMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);
            await InputQueueSendEndpoint.Send(new PongMessage(pingMessage.CorrelationId), x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> pingFaultContext = await pingFault;
            ConsumeContext<Fault<PongMessage>> pongFaultContext = await pongFault;

            Assert.That(_consumer.PingCount, Is.EqualTo(3));
            Assert.That(_consumer.PongCount, Is.EqualTo(3));
        }

        Consumer _consumer;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            configurator.UseDelayedRedelivery(r => r.Intervals(100, 200));

            _consumer = new Consumer();
            configurator.Consumer(() => _consumer);
        }


        class Consumer :
            IConsumer<PingMessage>,
            IConsumer<PongMessage>
        {
            public int PingCount;
            public int PongCount;

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                Interlocked.Increment(ref PingCount);

                throw new IntentionalTestException();
            }

            public Task Consume(ConsumeContext<PongMessage> context)
            {
                Interlocked.Increment(ref PongCount);

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class Using_delayed_exchange_redelivery_with_a_consumer_and_retry :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
    [Category("Flaky")]
    public class Delaying_a_message_retry_with_policy_but_no_retries :
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
    [Category("Flaky")]
    public class Explicitly_deferring_a_message_instead_of_throwing :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            var pingMessage = new PingMessage();
            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            var timer = Stopwatch.StartNew();

            await _received.Task;

            timer.Stop();

            Assert.That(timer.Elapsed, Is.GreaterThan(TimeSpan.FromSeconds(0.5)));
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        int _count;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_count++ == 0)
                {
                    await context.Defer(TimeSpan.FromMilliseconds(1000));
                    return;
                }

                _received.TrySetResult(context);
            });
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class Execute_callback_function_during_defer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_execute_callback_during_defer_the_message_delivery()
        {
            var pingMessage = new PingMessage();
            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            await _received.Task;

            Assert.IsTrue(_hit);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        int _count;
        bool _hit;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_count++ == 0)
                {
                    await context.Defer(TimeSpan.FromMilliseconds(100), (consumeContext, sendContext) =>
                    {
                        _hit = true;
                    });

                    return;
                }

                _received.TrySetResult(context);
            });
        }
    }
}
