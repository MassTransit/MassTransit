namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Policies;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_delayed_exchange :
        RabbitMqTestFixture
    {
        [Test]
        public async void Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
            }, x => x.UseDelayedRedelivery(Retry.Intervals(1000, 2000)));
        }
    }

    [TestFixture]
    public class Explicitly_deferring_a_message_instead_of_throwing :
        RabbitMqTestFixture
    {
        [Test]
        public async void Should_properly_defer_the_message_delivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _received.Task;

            Assert.GreaterOrEqual(_receivedTimeSpan, TimeSpan.FromSeconds(1));
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _received;
        TimeSpan _receivedTimeSpan;
        Stopwatch _timer;
        int _count;

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
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
