namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;

	public class Batch<TMessage, TBatchId> :
		Consumes<TMessage>.All,
		BatchedBy<TBatchId>,
		IEnumerable<TMessage>
		where TMessage : class, BatchedBy<TBatchId>
	{
		private readonly TBatchId _batchId;
		private readonly int _batchLength;
		private readonly ManualResetEvent _complete = new ManualResetEvent(false);
		private readonly Semaphore _messageReady;
		private readonly Queue<TMessage> _messages = new Queue<TMessage>();
		private readonly TimeSpan _timeout;
		private int _messageCount;

		public Batch(int batchLength, TBatchId batchId, TimeSpan timeout)
		{
			_batchLength = batchLength;
			_batchId = batchId;
			_timeout = timeout;

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
			{
				_messages.Enqueue(message);
				_messageReady.Release();
			}
		}

		IEnumerator<TMessage> IEnumerable<TMessage>.GetEnumerator()
		{
			WaitHandle[] handles = new WaitHandle[] {_messageReady, _complete};

			int waitResult;
			while ((waitResult = WaitHandle.WaitAny(handles, _timeout, true)) == 0)
			{
				lock (_messages)
				{
					TMessage message = _messages.Dequeue();
					_messageCount++;

					if (_messageCount == _batchLength)
						_complete.Set();

					yield return message;
				}
			}

			if (waitResult == WaitHandle.WaitTimeout)
				throw new ApplicationException("Timeout waiting for batch to complete");
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<TMessage>) this).GetEnumerator();
		}
	}
}