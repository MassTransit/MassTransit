namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class MsmqStorage_Idempotent_Tests
    {
        [Test]
        [Explicit("needs items in the queue to work - dru")]
        public void Add_Should_Be_Idempotent()
        {
            IMessageQueueEndpoint ep = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");
            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage store = new MsmqSubscriptionStorage(ep, cache);
            
            Assert.That(store.List().Count, Is.EqualTo(2));
        }
    }
}