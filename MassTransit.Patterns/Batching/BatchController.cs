namespace MassTransit.Patterns.Batching
{
    using System;
    using System.Collections.Generic;
    using ServiceBus;

    public class BatchController<T, K> :
        IMessage where T : BatchMessage<K>
    {
        private readonly Dictionary<K, BatchContext<T, K>> _contexts = new Dictionary<K, BatchContext<T, K>>();
        private readonly BatchControllerHandler<T, K> _handler;
        private readonly object _lockContext = new object();
        private readonly TimeSpan _timeout;

        public BatchController(BatchControllerHandler<T, K> handler)
        {
            _handler = handler;
            _timeout = TimeSpan.FromMinutes(30);
        }

        public BatchController(BatchControllerHandler<T, K> handler, TimeSpan timeout)
        {
            _handler = handler;
            _timeout = timeout;
        }

        public void HandleMessage(IMessageContext<T> context)
        {
            K batchId = context.Message.BatchId;

            BatchContext<T, K> batchContext;

            bool invokeHandler = false;

            lock (_lockContext)
            {
                // if we have an existing context for the batchId of this message
                if (_contexts.ContainsKey(batchId))
                {
                    batchContext = _contexts[batchId];
                }
                else
                {
                    // we don't have a context for this one yet, so create one
                    batchContext = new BatchContext<T, K>(context, batchId, _timeout);

                    _contexts.Add(batchId, batchContext);

                    invokeHandler = true;
                }
            }

            // push this message to the context, releasing the enumerator
            batchContext.Enqueue(context.Message);

            if (invokeHandler)
            {
                _handler(batchContext);
            }
        }
    }

    public delegate void BatchControllerHandler<T, K>(BatchContext<T, K> context) where T : BatchMessage<K>;
}