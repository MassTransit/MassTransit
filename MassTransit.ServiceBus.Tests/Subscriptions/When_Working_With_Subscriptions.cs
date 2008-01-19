namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_Working_With_Subscriptions
    {
        private MockRepository mocks;
        string mockPath = "msmq://localhost/bob";

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
        }


        [Test]
        public void Add_Subscription_without_a_bus()
        {
            LocalSubscriptionCache cache = new LocalSubscriptionCache();


            cache.Add(typeof (PingMessage).FullName, new Uri(mockPath));

            Assert.That(cache.List().Count, Is.EqualTo(1));
        }
    }
}