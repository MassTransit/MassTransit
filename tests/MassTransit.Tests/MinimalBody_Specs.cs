namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_consuming_a_minimal_message_body :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_use_the_correct_intervals_for_each_redelivery()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), x => x.Serializer = new CopyBodySerializer(SystemTextJsonMessageSerializer.JsonContentType,
                new StringMessageBody(@"
{
    ""message"": {
            ""CorrelationId"": ""9BBCF5F0-596E-4E90-B3C5-1B27358C5AEB""
        },
        ""messageType"": [
        ""urn:message:MassTransit.TestFramework.Messages:PingMessage""
        ]
    }
")));

            await Task.WhenAll(_received.Select(x => x.Task));

            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));

            TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
        }

        TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
        int _count;
        DateTime[] _timestamps;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _count = 0;
            _received = new[] { GetTask<ConsumeContext<PingMessage>>(), GetTask<ConsumeContext<PingMessage>>() };
            _timestamps = new DateTime[2];

            configurator.UseDelayedRedelivery(r => r.Intervals(1000));

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
