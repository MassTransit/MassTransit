namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_scheduled_delay_retry_mechanism :
        QuartzInMemoryTestFixture
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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseMessageScheduler(QuartzAddress);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
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
            }, x => x.UseScheduledRedelivery(r => r.Intervals(1000, 2000)));
        }
    }


    [TestFixture]
    public class Using_a_scheduled_delay_retry_mechanism_for_consumer :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _consumer.Received;

            Assert.GreaterOrEqual(_consumer.ReceivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        MyConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<ConsumeContext<PingMessage>>());

            configurator.Consumer(() => _consumer, x =>
            {
                x.Message<PingMessage>(m => m.UseScheduledRedelivery(r => r.Intervals(1000, 2000)));
            });
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _received;
            int _count;
            TimeSpan _receivedTimeSpan;
            Stopwatch _timer;

            public MyConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _received = taskCompletionSource;
            }

            public Task<ConsumeContext<PingMessage>> Received => _received.Task;

            public IComparable ReceivedTimeSpan => _receivedTimeSpan;

            public Task Consume(ConsumeContext<PingMessage> context)
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
            }
        }
    }


    [TestFixture]
    public class Using_a_scheduled_delay_retry_mechanism_for_consumer_without_message :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _consumer.Received;

            Assert.GreaterOrEqual(_consumer.ReceivedTimeSpan, TimeSpan.FromSeconds(1));

            Assert.That(_consumer.RedeliveryCount, Is.EqualTo(2));
        }

        MyConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<ConsumeContext<PingMessage>>());

            configurator.UseScheduledRedelivery(r => r.Intervals(1000, 2000));
            configurator.Consumer(() => _consumer);
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _received;
            int _count;
            TimeSpan _receivedTimeSpan;
            Stopwatch _timer;

            public MyConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _received = taskCompletionSource;
            }

            public Task<ConsumeContext<PingMessage>> Received => _received.Task;

            public IComparable ReceivedTimeSpan => _receivedTimeSpan;

            public int RedeliveryCount { get; set; }

            public Task Consume(ConsumeContext<PingMessage> context)
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
                RedeliveryCount = context.GetRedeliveryCount();
                _received.TrySetResult(context);

                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class Using_scheduled_redelivery_configured_via_the_bus :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_order_the_middleware()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _consumer.Received;

            Assert.GreaterOrEqual(_consumer.ReceivedTimeSpan, TimeSpan.FromSeconds(1));

            Assert.That(_consumer.RedeliveryCount, Is.EqualTo(2));
            Assert.That(context.GetRedeliveryCount(), Is.EqualTo(2));
        }

        MyConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseScheduledRedelivery(r => r.Intervals(1000, 2000));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new MyConsumer(GetTask<ConsumeContext<PingMessage>>());

            configurator.Consumer(() => _consumer);
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _received;
            int _count;
            TimeSpan _receivedTimeSpan;
            Stopwatch _timer;

            public MyConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _received = taskCompletionSource;
            }

            public Task<ConsumeContext<PingMessage>> Received => _received.Task;

            public IComparable ReceivedTimeSpan => _receivedTimeSpan;

            public int RedeliveryCount { get; set; }

            public Task Consume(ConsumeContext<PingMessage> context)
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
                RedeliveryCount = context.GetRedeliveryCount();
                _received.TrySetResult(context);

                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class Using_an_explicit_retry_later_via_scheduling :
        QuartzInMemoryTestFixture
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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseMessageScheduler(QuartzAddress);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_timer == null)
                    _timer = Stopwatch.StartNew();

                if (_count++ < 2)
                {
                    Console.WriteLine("{0} now is not a good time", DateTime.UtcNow);
                    await context.Redeliver(TimeSpan.FromMilliseconds(1000));
                    return;
                }

                _timer.Stop();

                Console.WriteLine("{0} okay, now is good (retried {1} times)", DateTime.UtcNow, context.GetRedeliveryCount());

                // okay, ready.
                _receivedTimeSpan = _timer.Elapsed;
                _received.TrySetResult(context);
            });
        }
    }


    [TestFixture]
    public class Using_an_explicit_retry_later_via_scheduling_with_custom_callback :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task callback_executed_before_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
            var customHeaderValue = context.Headers.Get(customHeader, default(int?));
            Assert.AreEqual(2, customHeaderValue);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;
        readonly string customHeader = "Custom-Header";

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;

            _received = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(async context =>
            {
                if (_timer == null)
                    _timer = Stopwatch.StartNew();

                if (_count++ < 2)
                {
                    Console.WriteLine("{0} now is not a good time", DateTime.UtcNow);
                    await context.Redeliver(TimeSpan.FromMilliseconds(1000), (consumeContext, sendContext) =>
                    {
                        sendContext.Headers.Set(customHeader, 2);
                    });

                    return;
                }

                _timer.Stop();

                Console.WriteLine("{0} okay, now is good (retried {1} times)", DateTime.UtcNow, context.GetRedeliveryCount());

                // okay, ready.
                _receivedTimeSpan = _timer.Elapsed;
                _received.TrySetResult(context);
            });
        }
    }
}
