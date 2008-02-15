namespace MassTransit.Patterns.Tests
{
    using System;
    using Batching;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;

    [TestFixture]
    public class As_a_BatchController
    {
        [Test]
        public void Hmm()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());

            bool wasCalled = false;
            bool isComplete = false;

            BatchController<MessageToBatch, Guid> c = new BatchController<MessageToBatch, Guid>(
                delegate(BatchContext<MessageToBatch, Guid> cxt)
                {
                    foreach (MessageToBatch msg in cxt)
                    {
                        wasCalled = true;
                        isComplete = cxt.IsComplete;
                    }
                }, TimeSpan.FromSeconds(3));

            Guid batchId = Guid.NewGuid();
            int batchLength = 1;

            MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);

            IEnvelope env1 = new Envelope(msg1);

            bus.Subscribe<MessageToBatch>(c.HandleMessage);

            bus.Deliver(env1);

            Assert.That(wasCalled, Is.True, "Not Called");
            Assert.That(isComplete, Is.True, "Not Complete");
            
        }
    }
}