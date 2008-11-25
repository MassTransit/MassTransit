namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using TestConsumers;

    public class TestBatchConsumer<TMessage, TBatchId> :
        TestConsumerBase<Batch<TMessage, TBatchId>>,
        Consumes<Batch<TMessage, TBatchId>>.Selected
        where TMessage : class, BatchedBy<TBatchId>
    {
        private static readonly List<TBatchId> _batchesReceived = new List<TBatchId>();
        private static readonly Semaphore _batchReceived = new Semaphore(0, 100);
        private readonly ManualResetEvent _completed = new ManualResetEvent(false);
        private int _messageCount;

        public override void Consume(Batch<TMessage, TBatchId> batch)
        {
            base.Consume(batch);

            foreach (IndividualBatchMessage message in batch)
            {
                Interlocked.Increment(ref _messageCount);
            }

            if (_messageCount == batch.BatchLength)
            {
                _batchesReceived.Add(batch.BatchId);
                _batchReceived.Release();

                _completed.Set();
            }
        }

        public bool Accept(Batch<TMessage, TBatchId> message)
        {
            return true;
        }

        public void ShouldHaveReceivedBatch(TimeSpan timeout)
        {
            Assert.That(_completed.WaitOne(timeout, true), Is.True, "The batch should have completed");
        }

        public void ShouldNotHaveCompletedBatch(TimeSpan timeout)
        {
            Assert.That(_completed.WaitOne(timeout, true), Is.False, "The batch should not have completed");
        }

        public static void AnyShouldHaveReceivedBatch(TBatchId batchId, TimeSpan timeout)
        {
            while (_batchesReceived.Contains(batchId) == false)
            {
                Assert.That(_batchReceived.WaitOne(timeout, true), Is.True, "The batch should have been received");
            }
        }
    }
}