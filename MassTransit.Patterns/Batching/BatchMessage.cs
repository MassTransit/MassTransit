namespace MassTransit.Patterns.Batching
{
    using System;
    using ServiceBus;


    //abstract?
    //IBatchMessage?
    //Batch<IMessage>?
    [Serializable]
    public class BatchMessage<T> : IMessage where T : IMessage
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