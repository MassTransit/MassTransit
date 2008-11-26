namespace MassTransit.Transports.Wcf.Tests
{
    using System;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using MassTransit.Tests.TestConsumers;
    using NUnit.Framework;

    [TestFixture]
    public class When_a_message_is_published :
        LocalAndRemoteTestContext
    {
        protected override string GetCastleConfigurationFile()
        {
            return "wcf.castle.xml";
        }

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.Subscribe(consumer);

            PingMessage message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }
    }
}