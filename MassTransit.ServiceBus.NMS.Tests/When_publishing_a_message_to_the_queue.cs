namespace MassTransit.ServiceBus.NMS.Tests
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Subscriptions;

    [TestFixture]
    public class When_publishing_a_message_to_the_queue
    {
        [Test]
        public void The_Message_Should_Arrive()
        {
            ServiceBus bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/published_queue"), new LocalSubscriptionCache());
            bus.SubscriptionStorage.Add(typeof(SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue"));
            bus.Publish(new SimpleMessage("dru"));
        }

        [Test]
        public void The_Message_Should_Be_Consumed()
        {
            ServiceBus pub_bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/published_queue"), new LocalSubscriptionCache());
            ServiceBus sub_bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/subscribed_queue"), new LocalSubscriptionCache());
            pub_bus.SubscriptionStorage.Add(typeof(SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue"));
            sub_bus.SubscriptionStorage.Add(typeof(SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue"));

            ManualResetEvent received = new ManualResetEvent(false);

            bool wasCalled = false;
            sub_bus.Subscribe<SimpleMessage>(delegate
                                                 {
                                                     wasCalled = true;
                                                     received.Set();
                                                 });
            
            pub_bus.Publish(new SimpleMessage("dru"));

            Assert.That(received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);
            Assert.IsTrue(wasCalled);


        }
    }
}