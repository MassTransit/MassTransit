namespace HeavyLoad
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using log4net;

	/// <summary>
	/// A managed collection of threads for handling tasks
	/// </summary>
	public class ManagedThreadPool<T> : IDisposable
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ManagedThreadPool<T>));
		readonly Action<T> _handler;
		readonly AutoResetEvent _queueActivity = new AutoResetEvent(true);
		readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		readonly List<Thread> _threads = new List<Thread>();
		readonly Semaphore _workAvailable = new Semaphore(0, int.MaxValue);
		readonly Queue<T> _workItems = new Queue<T>();
		int _maxQueueDepth = 5000;
		int _maxThreads;
		int _minThreads;

		public ManagedThreadPool(Action<T> handler)
			: this(handler, 1, 10)
		{
		}

		public ManagedThreadPool(Action<T> handler, int minThreads, int maxThreads)
		{
			_handler = handler;
			_minThreads = minThreads;
			_maxThreads = maxThreads;
		}

		/// <summary>
		/// The maximum depth of the queue
		/// </summary>
		public int MaxQueueDepth
		{
			[DebuggerStepThrough]
			get { return _maxQueueDepth; }
		}

		public int MaxThreads
		{
			[DebuggerStepThrough]
			get { return _maxThreads; }
			set
			{
				_maxThreads = value;
			}
		}

		public int MinThreads
		{
			[DebuggerStepThrough]
			get { return _minThreads; }
			set
			{

				_minThreads = value;
			}
		}

		public int CurrentThreadCount
		{
			[DebuggerStepThrough]
			get { lock (_threads) return _threads.Count; }
		}

		public int QueueDepth
		{
			[DebuggerStepThrough]
			get { lock (_workItems) return _workItems.Count; }
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

		public void Enqueue(T item)
		{
			WaitForRoomInQueue();

			lock (_workItems)
				_workItems.Enqueue(item);
			_workAvailable.Release();

			AdjustQueueCount();
		}

		T Dequeue()
		{
			T item;
			lock (_workItems)
				item = _workItems.Dequeue();

			_queueActivity.Set();

			return item;
		}

		void WaitForRoomInQueue()
		{
			while (QueueDepth >= MaxQueueDepth)
			{
				_queueActivity.WaitOne(TimeSpan.FromSeconds(10), true);
			}
		}

		void AdjustQueueCount()
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

		void AddThread()
		{
			var thread = new Thread(RunThread);

			lock (_threads)
				_threads.Add(thread);

			thread.Start();

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Created Thread {0}, Total Threads = {1}", thread.ManagedThreadId, CurrentThreadCount);
		}

		void RunThread(object obj)
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting Thread {0}", threadId);

			var handles = new WaitHandle[] {_shutdown, _workAvailable};

			int result;
			while ((result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(5), true)) != 0)
			{
				if (result == WaitHandle.WaitTimeout)
				{
					lock (_threads)
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