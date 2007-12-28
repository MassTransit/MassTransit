namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;

    [TestFixture]
    public class When_Working_With_Subscriptions :
        ServiceBusSetupFixture
    {

        [Test]
        public void Subscriptions_Should_Collect()
        {
            MsmqSubscriptionStorage subs = new MsmqSubscriptionStorage(base._subscriptionQueueName, null, base._subscriptionCache);
        }
    }
}