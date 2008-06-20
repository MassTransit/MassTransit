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
namespace MassTransit.ServiceBus.Threading
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using log4net;
	using Util;

	/// <summary>
	/// A managed collection of threads for handling tasks
	/// </summary>
	public class ManagedThreadPool<T> : IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ManagedThreadPool<T>));
		private readonly Action<T> _handler;
		private readonly AutoResetEvent _queueActivity = new AutoResetEvent(true);
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly List<Thread> _threads = new List<Thread>();
		private readonly Semaphore _workAvailable = new Semaphore(0, int.MaxValue);
		private readonly Queue<T> _workItems = new Queue<T>();
		private int _maxThreads;
		private int _minThreads;

		public ManagedThreadPool(Action<T> handler)
			: this(handler, 1, 50)
		{
		}

		public ManagedThreadPool(Action<T> handler, int minThreads, int maxThreads)
		{
			Check.Ensure(minThreads >= 0, "The minimum thread count must be greater than zero");
			Check.Ensure(maxThreads >= minThreads, "The maximum thread count must be at least equal to the minimum thread count");
			Check.Parameter(handler).WithMessage("The handler must not be null").IsNotNull();

			_handler = handler;
			_minThreads = minThreads;
			_maxThreads = maxThreads;
		}

		public void Dispose()
		{
			// let's eat up any outstanding messages if possible
			for (int i = 0; i < 60; i++)
			{
				if (QueueDepth == 0)
					break;

				Thread.Sleep(1000);
			}

			_shutdown.Set();

			Thread[] remaining = _threads.ToArray();

			foreach (Thread t in remaining)
			{
				if (!t.Join(TimeSpan.FromSeconds(60)))
				{
					// TODO log message that thread did not exit properly
				}
			}

			_threads.Clear();
		}

		/// <summary>
		/// The maximum depth of the queue
		/// </summary>
		public int MaxQueueDepth
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _maxThreads * 2; }
		}

		public int MaxThreads
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _maxThreads; }
			set
			{
				Check.Ensure(value >= _minThreads, "The maximum thread count must be at least equal to the minimum thread count");

				_maxThreads = value;
			}
		}

		public int MinThreads
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _minThreads; }
			set
			{
				Check.Ensure(value >= 0, "The minimum thread count must be greater than zero");
				Check.Ensure(_maxThreads >= value, "The maximum thread count must be at least equal to the minimum thread count");

				_minThreads = value;
			}
		}

		public int CurrentThreadCount
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { lock(_threads) return _threads.Count; }
		}

		public int QueueDepth
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { lock (_workItems) return _workItems.Count; }
		}

		public void Enqueue(T item)
		{
			WaitForRoomInQueue();

			lock (_workItems)
				_workItems.Enqueue(item);
			_workAvailable.Release();

			AdjustQueueCount();
		}

		private T Dequeue()
		{
			T item;
			lock (_workItems)
				item = _workItems.Dequeue();

			_queueActivity.Set();

			return item;
		}

		private void WaitForRoomInQueue()
		{
			while (QueueDepth >= MaxQueueDepth)
			{
				_queueActivity.WaitOne(TimeSpan.FromSeconds(10), true);
			}
		}

		private void AdjustQueueCount()
		{
			int queueCount = QueueDepth;

			// if the previous count is greater than zero, we have work waiting to be picked up
			if (queueCount > 0 && CurrentThreadCount < MaxThreads)
			{
				AddThread();
			}

			// build the thread pool up to minimum
			while (CurrentThreadCount < MinThreads)
			{
				AddThread();
			}
		}

		private void AddThread()
		{
			Thread thread = new Thread(RunThread);

			lock (_threads)
				_threads.Add(thread);
			
			thread.Start();

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Created Thread {0}, Total Threads = {1}", thread.ManagedThreadId, CurrentThreadCount);
		}

		private void RunThread(object obj)
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;

			if(_log.IsDebugEnabled)
				_log.DebugFormat("Starting Thread {0}", threadId);

			WaitHandle[] handles = new WaitHandle[] {_shutdown, _workAvailable};

			int result;
			while ((result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(5), true)) != 0)
			{
				if (result == WaitHandle.WaitTimeout)
				{
					lock(_threads)
					{
						if (CurrentThreadCount > MinThreads)
						{
							_threads.Remove(Thread.CurrentThread);
							break;
						}
					}

					continue;
				}

				try
				{
					T item = Dequeue();

					_handler(item);
				}
				catch (Exception ex)
				{
					_log.Error("An exception occurred processing an item of type: " + typeof (T).FullName, ex);
				}
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("ExitingThread {0}", threadId);
		}
	}
}