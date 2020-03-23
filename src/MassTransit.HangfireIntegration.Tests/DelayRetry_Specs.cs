// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Using_a_scheduled_delay_retry_mechanism :
        HangfireInMemoryTestFixture
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

            configurator.UseMessageScheduler(HangfireAddress);
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
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _consumer.Received;

            Assert.GreaterOrEqual(_consumer.ReceivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        MyConsumer _consumer;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseMessageScheduler(HangfireAddress);
        }

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

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Using_a_scheduled_delay_retry_mechanism_for_consumer_without_message :
        HangfireInMemoryTestFixture
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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseMessageScheduler(HangfireAddress);
        }

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
            int _redeliveryCount;

            public MyConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _received = taskCompletionSource;
            }

            public Task<ConsumeContext<PingMessage>> Received => _received.Task;

            public IComparable ReceivedTimeSpan => _receivedTimeSpan;

            public int RedeliveryCount => _redeliveryCount;

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
                _redeliveryCount = context.GetRedeliveryCount();
                _received.TrySetResult(context);

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Using_an_explicit_retry_later_via_scheduling :
        HangfireInMemoryTestFixture
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

            configurator.UseMessageScheduler(HangfireAddress);
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
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task callback_executed_before_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
            int? customHeaderValue = context.Headers.Get(customHeader, default(int?));
            Assert.AreEqual(2, customHeaderValue);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;
        string customHeader = "Custom-Header";

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseMessageScheduler(HangfireAddress);
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
