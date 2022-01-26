namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_scheduled_redelivery :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_use_the_correct_intervals_for_each_redelivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await Task.WhenAll(_received.Select(x => x.Task));

            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(3)));

            TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
        int _count;
        DateTime[] _timestamps;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;
            _received = new[]
            {
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>()
            };
            _timestamps = new DateTime[4];

            configurator.UseScheduledRedelivery(r => r.Intervals(1000, 2000, 3000));

            configurator.Handler<PingMessage>(async context =>
            {
                _received[_count].TrySetResult(context);
                _timestamps[_count] = DateTime.Now;

                _count++;

                throw new IntentionalTestException("I'm so not ready for this jelly.");
            });
        }
    }
}
