using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
    using System.Messaging;
    using MassTransit.ServiceBus.Subscriptions;

    [TestFixture]
    [Explicit]
    public class When_sending_a_message
    {
        [Test]
        [Ignore("No Test")]
        public void The_message_should_be_delivered_to_a_local_subscriber()
        {
        }

        [Test]
        public void NAME()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test_client"), new LocalSubscriptionCache());
            bus.Send(new MessageQueueEndpoint("msmq://localhost/test"), new PingMessage());
        }
    }
}