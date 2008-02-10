namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_a_new_batch_is_received
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mocks = new MockRepository();
            _bus = _mocks.CreateMock<IServiceBus>();
        }

        #endregion

        private MockRepository _mocks;
        private IServiceBus _bus;

        [Test]
        public void Notify_the_subscriber_with_a_batch_message()
        {
            BatchTransferController<BatchMessage, Guid> controller = new BatchTransferController<BatchMessage, Guid>(_bus,
                delegate(IMessageContext<BatchTransferController<BatchMessage, Guid>> context)
                    {
                        Guid batchId = context.Message.BatchId;

                        foreach (BatchTransferMessage<Guid> message in context.Message)
                        {
                        }
                    });
        }
    }

    [Serializable]
    public abstract class BatchTransferMessage<K> :
        IMessage
    {
        private K _batchId;
        private long _batchLength;

        /// <summary>
        /// Identifies the batch containing this message
        /// </summary>
        public K BatchId
        {
            get { return _batchId; }
            set { _batchId = value; }
        }

        /// <summary>
        /// The number of messages in the batch
        /// </summary>
        public long BatchLength
        {
            get { return _batchLength; }
            set { _batchLength = value; }
        }
    }

    [Serializable]
    public class BatchTransferController<T, K> :
        IEnumerable<T>,
        IMessage where T : BatchTransferMessage<K>
    {
        private readonly IServiceBus _bus;
        private readonly MessageReceivedCallback<T> _context;
        private readonly ManualResetEvent _complete = new ManualResetEvent(false);
        private readonly Semaphore _messageReady = new Semaphore(0, 0);
        private readonly Queue<T> _messages = new Queue<T>();

        private K _batchId;

        public BatchTransferController(IServiceBus bus, MessageReceivedCallback<T> context)
        {
            _bus = bus;
            _context = context;

            _bus.Subscribe<T>(HandleMessage);
        }

        public K BatchId
        {
            get { return _batchId; }
            set { _batchId = value; }
        }

        public void HandleMessage(IMessageContext<T> context)
        {
            if(context.Message.BatchId.Equals(_batchId))
            {
                // this is for our batch
                
            }
            else
            {
                // this is a new batch, we need to create a new controller and subscribe with the appropriate predicate to handle it
            }
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            WaitHandle[] handles = new WaitHandle[] {_messageReady, _complete};
            int waitResult;
            while ((waitResult = WaitHandle.WaitAny(handles, TimeSpan.FromMinutes(10), true)) != 1)
            {
                T message = _messages.Dequeue();

                yield return message;
            }

            if (waitResult == WaitHandle.WaitTimeout)
                throw new ApplicationException("Timeout waiting for batch to complete");
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }

        #endregion
    }

    [Serializable]
    public class BatchMessage : 
        BatchTransferMessage<Guid>
    {
        
    }

    [Serializable]
    public class BatchTransferRequest :
        BatchTransferController<BatchTransferMessage<Guid>, Guid>
    {
    }
}