/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.Patterns.Batching
{
    using System;
    using System.Collections.Generic;
    using ServiceBus;

    public class BatchController2<T, K> where T : IBatchMessage
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