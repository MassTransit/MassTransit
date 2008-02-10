namespace MassTransit.Patterns.Tests
{
    using Batching;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    using ServiceBus;

    [TestFixture]
    public class BatchingPattern
    {
        private MockRepository mocks;

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
        public void Getting_Them_All()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());
            BatchManager mgr = new BatchManager();
            BatchMessage<MessageToBatch> msg1 = new BatchMessage<MessageToBatch>(2, "dru", new MessageToBatch());
            BatchMessage<MessageToBatch> msg2 = new BatchMessage<MessageToBatch>(2, "dru", new MessageToBatch());

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<BatchMessage<MessageToBatch>>(delegate(IMessageContext<BatchMessage<MessageToBatch>> cxt)
                              {
                                  mgr.Enqueue(cxt.Message);
                              });
            bus.Deliver(env1);
            bus.Deliver(env2);

            Assert.That(mgr.BatchComplete, Is.True);
        }

        [Test]
        public void Getting_Them_All_but_one()
        {
            ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test"), new LocalSubscriptionCache());
            BatchManager mgr = new BatchManager();
            BatchMessage<MessageToBatch> msg1 = new BatchMessage<MessageToBatch>(3, "dru", new MessageToBatch());
            BatchMessage<MessageToBatch> msg2 = new BatchMessage<MessageToBatch>(3, "dru", new MessageToBatch());

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<BatchMessage<MessageToBatch>>(delegate(IMessageContext<BatchMessage<MessageToBatch>> cxt)
                              {
                                  mgr.Enqueue(cxt.Message);
                              });

            bus.Deliver(env1);
            bus.Deliver(env2);

            Assert.That(mgr.BatchComplete, Is.False);
        }
    }
}
