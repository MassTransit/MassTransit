namespace MassTransit.Patterns.Batching
{
    using System;
    using System.Collections.Generic;
    using ServiceBus;

    public class BatchController2<T, K> :
        IMessage where T : IBatchMessage
    {
        private readonly Dictionary<K, BatchContext2<T, K>> _contexts = new Dictionary<K, BatchContext2<T, K>>();
        private readonly BatchControllerHandler<T, K> _handler;
        private readonly object _lockContext = new object();
        private readonly TimeSpan _timeout;

        public BatchController2(BatchControllerHandler<T, K> handler)
        {
            _handler = handler;
            _timeout = TimeSpan.FromMinutes(30);
        }

        public BatchController2(BatchControllerHandler<T, K> handler, TimeSpan timeout)
        {
            _handler = handler;
            _timeout = timeout;
        }

        public void HandleMessage(IMessageContext<T> context)
        {
            K batchId = (K)context.Message.BatchId;

            BatchContext2<T, K> batchContext;

            lock (_lockContext)
            {
				if (!_contexts.ContainsKey(batchId))
				{
					batchContext = new BatchContext2<T, K>(context, batchId, _timeout);

					_contexts.Add(batchId, batchContext);

				}
				else
				{
					batchContext = _contexts[batchId];
				}

            }

            // push this message to the context, releasing the enumerator
            batchContext.Enqueue(context.Message);

            if (batchContext.IsComplete)
            {
                _handler(batchContext);
            }
        }
    }
}