namespace MassTransit.Patterns.Tests
{
    using System;
    using System.Collections.Generic;
    using Batching;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    using ServiceBus;

    [TestFixture]
    public class BatchingPattern
    {
        #region Setup/Teardown

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

        #endregion

        private MockRepository mocks;

        [Test]
        public void Getting_Them_All()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());
            
            List<MessageToBatch> messages = new List<MessageToBatch>();
            
            int i = 0;
            bool isComplete = false;
            bool wasCalled = false;

            BatchController<MessageToBatch, Guid> controller = new BatchController<MessageToBatch, Guid>(
                delegate(BatchContext<MessageToBatch, Guid> cxt)
                {
                    foreach (MessageToBatch msg in cxt)
                    {
                        wasCalled = true;
                        isComplete = cxt.IsComplete;
                        i++;
                    }
                }, TimeSpan.FromSeconds(3));

            Guid batchId = Guid.NewGuid();
            int batchLength = 2;

            MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);
            MessageToBatch msg2 = new MessageToBatch(batchId, batchLength);

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<MessageToBatch>(controller.HandleMessage);

            bus.Deliver(env1);
            bus.Deliver(env2);

            Assert.That(isComplete, Is.True, "Complete");
            Assert.That(wasCalled, Is.True, "was called");
            Assert.That(i, Is.EqualTo(2));
        }

        [Test]
        public void Getting_Them_All_but_one()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());

            List<MessageToBatch> messages = new List<MessageToBatch>();

            bool? complete = null;

            BatchController<MessageToBatch, Guid> controller = new BatchController<MessageToBatch, Guid>(
                delegate(BatchContext<MessageToBatch, Guid> context)
                    {
                        messages.AddRange(context);

                        complete = context.IsComplete;
                    }, TimeSpan.FromSeconds(3));

            Guid batchId = Guid.NewGuid();
            int batchLength = 3;

            MessageToBatch msg1 = new MessageToBatch(batchId, batchLength);
            MessageToBatch msg2 = new MessageToBatch(batchId, batchLength);

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<MessageToBatch>(controller.HandleMessage);

            bus.Deliver(env1);
            bus.Deliver(env2);

            Assert.That(messages.Count, Is.EqualTo(2));

            Assert.That(complete, Is.Not.Null);

            Assert.That(complete.Value, Is.False);
        }
    }

    [Serializable]
    public class MessageToBatch :
        BatchMessage<Guid>
    {
        public MessageToBatch(Guid batchId, int batchLength)
            : base(batchId, batchLength)
        {
        }
    }
}