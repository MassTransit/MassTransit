namespace MassTransit.Transports.Wcf.Tests
{
	using Magnum.DateTimeExtensions;
	using MassTransit.Tests.Messages;
    using MassTransit.Tests.TestConsumers;
    using NUnit.Framework;
    using TestFixtures;

    [TestFixture]
    public class When_a_message_is_published :
        WcfEndpointTestFixture
    {
        [Test]
        [Ignore("need to figure out how to pass in config file info")]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.Subscribe(consumer);

            PingMessage message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, 10.Seconds());
        }
    }
}