namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_delayed_redelivery :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_the_correct_intervals_for_each_redelivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await Task.WhenAll(_received.Select(x => x.Task));

            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));

            TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
        int _count;
        DateTime[] _timestamps;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseNewtonsoftJsonSerializer();

            _count = 0;
            _received = new[]
            {
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>(),
                GetTask<ConsumeContext<PingMessage>>()
            };
            _timestamps = new DateTime[4];

            configurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 3000));

            configurator.Handler<PingMessage>(async context =>
            {
                _received[_count].TrySetResult(context);
                _timestamps[_count] = DateTime.Now;

                _count++;

                throw new IntentionalTestException("I'm so not ready for this jelly.");
            });
        }
    }


    [TestFixture]
    public class Using_delayed_redelivery_with_system_text_json :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_the_correct_intervals_for_each_redelivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await Task.WhenAll(_received.Select(x => x.Task));

            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));

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

            configurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 3000));

            configurator.Handler<PingMessage>(async context =>
            {
                _received[_count].TrySetResult(context);
                _timestamps[_count] = DateTime.Now;

                _count++;

                throw new IntentionalTestException("I'm so not ready for this jelly.");
            });
        }
    }

    [TestFixture]
    public class Using_delayed_redelivery_with_new_message_id :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_the_correct_intervals_for_each_redelivery()
        {
            Guid messageId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new PingMessage(), x => x.MessageId = messageId);

            await Task.WhenAll(_received.Select(x => x.Task));

            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));

            TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
            TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);

            Assert.That(_received[0].Task.Result.MessageId.Value, Is.EqualTo(messageId));
            Assert.That(_received[1].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));
            Assert.That(_received[2].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));
            Assert.That(_received[3].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));

            Assert.That(_received[1].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
            Assert.That(_received[2].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
            Assert.That(_received[3].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
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

            configurator.UseDelayedRedelivery(r =>
            {
                r.ReplaceMessageId = true;
                r.Intervals(1000, 2000, 3000);
            });

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
