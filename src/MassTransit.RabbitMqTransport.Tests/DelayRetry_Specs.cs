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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
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

                return TaskUtil.Completed;
            }, x => x.UseDelayedRedelivery(r => r.Intervals(1000, 2000)));
        }
    }

    [TestFixture]
    public class Delaying_a_message_retry_with_policy :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_only_defer_up_to_the_retry_count()
        {
            var pingMessage = new PingMessage();

            var fault = SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(3));
        }

        int _count;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
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
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_only_retry_up_to_the_retry_count()
        {
            var pingMessage = new PingMessage();

            var fault = SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(3));
        }

        int _count;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _count = 0;

            configurator.Handler<PingMessage>(context =>
            {
                Interlocked.Increment(ref _count);

                throw new IntentionalTestException();
            }, x => x.UseRetry(r => r.Intervals(100, 200)));
        }
    }
    
    [TestFixture]
    public class Delaying_a_message_retry_with_policy_but_no_retries :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_immediately_fault_with_no_delay()
        {
            var pingMessage = new PingMessage();

            var fault = SubscribeHandler<Fault<PingMessage>>(x => x.Message.Message.CorrelationId == pingMessage.CorrelationId);

            await InputQueueSendEndpoint.Send(pingMessage, x => x.FaultAddress = Bus.Address);

            ConsumeContext<Fault<PingMessage>> faultContext = await fault;

            Assert.That(_count, Is.EqualTo(1));
        }

        int _count;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
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
}