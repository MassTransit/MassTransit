namespace MassTransit.Patterns.Batching
{
    using System.Collections.Generic;

    public class BatchManager
    {
        //for intial test only
        public bool BatchComplete = false;

        private Dictionary<object, Queue<BatchMessage<MessageToBatch>>> _queues = new Dictionary<object, Queue<BatchMessage<MessageToBatch>>>();

        public void Enqueue(BatchMessage<MessageToBatch> msg)
        {
            if (!_queues.ContainsKey(msg.BatchId))
            {
                _queues.Add(msg.BatchId, new Queue<BatchMessage<MessageToBatch>>());
            }

            _queues[msg.BatchId].Enqueue(msg);

            AddedMessage(msg, _queues[msg.BatchId]);
        }

        public void AddedMessage(BatchMessage<MessageToBatch> msg, Queue<BatchMessage<MessageToBatch>> queue)
        {
            if (msg.BatchCount == queue.Count)
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
}