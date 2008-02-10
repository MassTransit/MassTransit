namespace MassTransit.ServiceBus.Tests
{
    using System;
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

    public class BatchManager
    {
        //for intial test only
        public bool BatchComplete = false;

        private Dictionary<object, Queue<BatchMessage<MessageToBatch>>> _queues = new Dictionary<object, Queue<BatchMessage<MessageToBatch>>>();

        public void Enqueue(BatchMessage<MessageToBatch> msg)
        {
            if(!_queues.ContainsKey(msg.BatchId))
            {
                _queues.Add(msg.BatchId, new Queue<BatchMessage<MessageToBatch>>());
            }

            _queues[msg.BatchId].Enqueue(msg);

            AddedMessage(msg, _queues[msg.BatchId]);
        }

        public void AddedMessage(BatchMessage<MessageToBatch> msg, Queue<BatchMessage<MessageToBatch>> queue)
        {
            if(msg.BatchCount == queue.Count)
            {
                QueueComplete(queue);
            }
        }
        
        //API for user
        public void QueueComplete(IEnumerable<BatchMessage<MessageToBatch>> queue)
        {
            //do stuff
            BatchComplete = true;
        }
        
    }

    //abstract?
    //IBatchMessage?
    //Batch<IMessage>?
    [Serializable]
    public class BatchMessage<T> : IMessage where T: IMessage
    {
        private int _batchCount;
        private object _batchId;
        private T _message;


        public BatchMessage(int batchCount, object batchId, T message)
        {
            _batchCount = batchCount;
            _message = message;
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

        public T Message
        {
            get { return _message; }
        }
    }

    [Serializable]
    public class MessageToBatch : IMessage
    {
        
    }
}