namespace MassTransit.ServiceBus.Tests
{
    using System.Collections.Generic;
    using Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

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
            BatchMessage msg1 = new BatchMessage(2, "dru");
            BatchMessage msg2 = new BatchMessage(2, "dru");

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<BatchMessage>(delegate(IMessageContext<BatchMessage> cxt)
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
            BatchMessage msg1 = new BatchMessage(3, "dru");
            BatchMessage msg2 = new BatchMessage(3, "dru");

            IEnvelope env1 = new Envelope(msg1);
            IEnvelope env2 = new Envelope(msg2);

            bus.Subscribe<BatchMessage>(delegate(IMessageContext<BatchMessage> cxt)
                              {
                                  mgr.Enqueue(cxt.Message);
                              });
            bus.Deliver(env1);
            bus.Deliver(env2);

            Assert.That(mgr.BatchComplete, Is.False);
        }
    }

    public class BatchManager
    {
        //for intial test only
        public bool BatchComplete = false;

        private Dictionary<object, Queue<BatchMessage>>  _queues = new Dictionary<object, Queue<BatchMessage>>();

        public void Enqueue(BatchMessage msg)
        {
            if(!_queues.ContainsKey(msg.BatchId))
            {
                _queues.Add(msg.BatchId, new Queue<BatchMessage>());
            }

            _queues[msg.BatchId].Enqueue(msg);

            AddedMessage(msg, _queues[msg.BatchId]);
        }

        public void AddedMessage(BatchMessage msg, Queue<BatchMessage> queue)
        {
            if(msg.BatchCount == queue.Count)
            {
                QueueComplete(queue);
            }
        }
        
        public void QueueComplete(Queue<BatchMessage> queue)
        {
            //do stuff
            BatchComplete = true;
        }
        
    }
    public class BatchMessage : IMessage
    {
        private int _batchCount;
        private object _batchId;


        public BatchMessage(int batchCount, object batchId)
        {
            _batchCount = batchCount;
            _batchId = batchId;
        }

        public int BatchCount
        {
            get { return _batchCount; }
        }

        public object BatchId
        {
            get { return _batchId; }
        }
    }
}