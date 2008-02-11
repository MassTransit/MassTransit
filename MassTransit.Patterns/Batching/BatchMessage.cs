namespace MassTransit.Patterns.Batching
{
    using System;
    using ServiceBus;

    [Serializable]
    public abstract class BatchMessage<K> :
        IMessage
    {
        private K _batchId;
        private int _batchLength;

        public BatchMessage(K batchId, int batchLength)
        {
            _batchId = batchId;
            _batchLength = batchLength;
        }

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
        public int BatchLength
        {
            get { return _batchLength; }
            set { _batchLength = value; }
        }
    }
}