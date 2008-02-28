namespace MassTransit.Patterns.Batching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using ServiceBus;

    public class BatchContext<T, K> :
        IEnumerable<T> where T : IBatchMessage
    {
        private readonly K _batchId;
        private readonly IServiceBus _bus;
        private readonly ManualResetEvent _complete = new ManualResetEvent(false);
        private readonly Semaphore _messageReady;
        private readonly Queue<T> _messages = new Queue<T>();
        private readonly IEndpoint _returnEndpoint;
        private readonly TimeSpan _timeout;
    	private int _messageCount;

    	public BatchContext(IMessageContext<T> context, K batchId, TimeSpan timeout)
        {
            _batchId = batchId;
            _timeout = timeout;
            _bus = context.Bus;
            _returnEndpoint = context.Envelope.ReturnEndpoint;
            _messageReady = new Semaphore(0, context.Message.BatchLength);
        }

        public K BatchId
        {
            get { return _batchId; }
        }

        public IEndpoint ReturnEndpoint
        {
            get { return _returnEndpoint; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
        }

        public bool IsComplete
        {
            get { return _complete.WaitOne(0, false); }
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            WaitHandle[] handles = new WaitHandle[] {_messageReady, _complete};

            int waitResult;
            while ((waitResult = WaitHandle.WaitAny(handles, _timeout, true)) == 0)
            {
				lock (_messages)
				{
					T message = _messages.Dequeue();
					_messageCount++;

					if(_messageCount == message.BatchLength)
						_complete.Set();

					yield return message;
				}
            }

            if (waitResult == WaitHandle.WaitTimeout)
                throw new ApplicationException("Timeout waiting for batch to complete");
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }

        #endregion

        public void Enqueue(T message)
        {
            lock (_messages)
            {
                _messages.Enqueue(message);
                _messageReady.Release();
            }
        }
    }
}