// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using log4net;
    using Threading;

    public class Batch<TMessage, TBatchId> :
        ManagedThread,
        Consumes<TMessage>.All,
        BatchedBy<TBatchId>,
        IEnumerable<TMessage>
        where TMessage : class, BatchedBy<TBatchId>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Batch<TMessage, TBatchId>));
        private readonly TBatchId _batchId;
        private readonly int _batchLength;
        private readonly IServiceBus _bus;
        private readonly ManualResetEvent _complete = new ManualResetEvent(false);
        private readonly Consumes<Batch<TMessage, TBatchId>>.Selected _consumer;
        private readonly Semaphore _messageReady;
        private readonly Queue<TMessage> _messages = new Queue<TMessage>();
        private readonly TimeSpan _timeout;
        private int _messageCount;

        public Batch(IServiceBus bus, TBatchId batchId, int batchLength, TimeSpan timeout, Consumes<Batch<TMessage, TBatchId>>.Selected consumer)
        {
            _batchLength = batchLength;
            _bus = bus;
            _batchId = batchId;
            _timeout = timeout;
            _consumer = consumer;

            _messageReady = new Semaphore(0, batchLength);
        }

        public bool IsComplete
        {
            get { return _complete.WaitOne(0, false); }
        }

        public TBatchId BatchId
        {
            get { return _batchId; }
        }

        public int BatchLength
        {
            get { return _batchLength; }
        }

        public void Consume(TMessage message)
        {
            lock (_messages)
                _messages.Enqueue(message);

            _messageReady.Release();
        }

        IEnumerator<TMessage> IEnumerable<TMessage>.GetEnumerator()
        {
            WaitHandle[] handles = new WaitHandle[] {_messageReady, _complete};

            // TODO This can hang on shutdown if we're waiting for a batch to finish, so we need to have a kill/cancel to shut it down
            int waitResult;
            while ((waitResult = WaitHandle.WaitAny(handles, _timeout, true)) == 0)
            {
                TMessage message;
                lock (_messages)
                    message = _messages.Dequeue();

                yield return message;

				if (Interlocked.Increment(ref _messageCount) == _batchLength)
					_complete.Set();
            }

            if (waitResult == WaitHandle.WaitTimeout)
            {
                _bus.Publish(new BatchTimeout<TMessage, TBatchId>(_batchId));
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<TMessage>) this).GetEnumerator();
        }

        protected override void RunThread(object obj)
        {
            try
            {
                _consumer.Consume(this);
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Batch " + typeof (Batch<TMessage, TBatchId>).FullName + ":" + BatchId, ex);
            }
        }
    }
}