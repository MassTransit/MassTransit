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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using Batch;
	using log4net;
	using Magnum.Common.DateTimeExtensions;
	using Magnum.Common.Threading;
	using Threading;

	public class BatchCombiner<TMessage, TBatchId> :
		ManagedThread,
		IMessageSink<TMessage>,
		Consumes<TMessage>.All,
		IEnumerable<TMessage>
		where TMessage : class, BatchedBy<TBatchId>
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (BatchCombiner<TMessage, TBatchId>));

		private readonly TBatchId _batchId;
		private readonly int _batchLength;
		private readonly Batch<TMessage, TBatchId> _batchMessage;
		private readonly Consumes<Batch<TMessage, TBatchId>>.All _consumer;
		private readonly TimeSpan _timeout = 30.Seconds();
		private ManualResetEvent _complete = new ManualResetEvent(false);
		private volatile bool _disposed;
		private int _messageCount;
		private Semaphore _messageRequested = new Semaphore(0, 1);
		private ReaderWriterLockedObject<Queue<TMessage>> _messages = new ReaderWriterLockedObject<Queue<TMessage>>(new Queue<TMessage>());
		private Semaphore _messageWaiting = new Semaphore(0, 1);


		public BatchCombiner(TBatchId batchId, int batchLength, Consumes<Batch<TMessage, TBatchId>>.All consumer)
		{
			_batchId = batchId;
			_batchLength = batchLength;
			_consumer = consumer;

			_batchMessage = new Batch<TMessage, TBatchId>(batchId, batchLength, this);
			Start();
		}

		public object BatchId
		{
			get { return _batchId; }
		}

		public void Consume(TMessage message)
		{
			_messages.WriteLock(x => x.Enqueue(message));
			_messageWaiting.Release();
		}

		public IEnumerator<TMessage> GetEnumerator()
		{
			_messageRequested.Release();

			WaitHandle[] handles = new WaitHandle[] {_messageWaiting, _complete};

			// TODO This can hang on shutdown if we're waiting for a batch to finish, so we need to have a kill/cancel to shut it down
			int waitResult;
			while ((waitResult = WaitHandle.WaitAny(handles, _timeout, true)) == 0)
			{
				yield return _messages.WriteLock(x => x.Dequeue());

				if (Interlocked.Increment(ref _messageCount) == _batchLength)
				{
					_complete.Set();
					break;
				}

				_messageRequested.Release();
			}

			if (waitResult == WaitHandle.WaitTimeout)
			{
				// TODO _bus.Publish(new BatchTimeout<TMessage, TBatchId>(_batchId));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			if (IsCompleted())
				yield break;

			if (_batchId.Equals(message.BatchId) && _messageRequested.WaitOne(_timeout, false))
				yield return this;
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			return true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_disposed) return;
			if (disposing)
			{
				_messageRequested.Close();
				_messageRequested = null;

				_messageWaiting.Close();
				_messageWaiting = null;

				_complete.Close();
				_complete = null;

				_messages.Dispose();
				_messages = null;
			}

			_disposed = true;
		}

		~BatchCombiner()
		{
			Dispose(false);
		}

		private bool IsCompleted()
		{
			return _complete.WaitOne(0, false);
		}

		protected override void RunThread(object obj)
		{
			try
			{
				_consumer.Consume(_batchMessage);
			}
			catch (Exception ex)
			{
				_log.Error("Exception in Batch " + typeof (Batch<TMessage, TBatchId>).FullName + ":" + _batchId, ex);
			}
		}
	}
}