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
		private readonly object _lockObj = new object();
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly List<Thread> _threads = new List<Thread>();
		private readonly Semaphore _workAvailable = new Semaphore(0, int.MaxValue);
		private readonly Queue<T> _workItems = new Queue<T>();
		private int _maxThreads;
		private int _minThreads;


		public ManagedThreadPool(Action<T> handler)
			: this(handler, 1, 256)
		{
		}

		public ManagedThreadPool(Action<T> handler, int minThreads, int maxThreads)
		{
			Check.Ensure(minThreads > 0, "The minimum thread count must be greater than zero");
			Check.Ensure(maxThreads >= minThreads, "The maximum thread count must be at least equal to the minimum thread count");
			Check.Parameter(handler).WithMessage("The handler must not be null").IsNotNull();

			_handler = handler;
			_minThreads = minThreads;
			_maxThreads = maxThreads;
		}

		public int MaxThreads
		{
			get { return _maxThreads; }
			set { _maxThreads = value; }
		}

		public int MinThreads
		{
			get { return _minThreads; }
			set { _minThreads = value; }
		}

		public int CurrentThreadCount
		{
			get { return _threads.Count; }
		}

		public int PendingCount
		{
			get { return _workItems.Count; }
		}

		public void Dispose()
		{
			while(PendingCount > 0)
				Thread.Sleep(10);

			_shutdown.Set();

			foreach (Thread t in _threads)
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
			lock (_workItems)
				_workItems.Enqueue(item);

			int count = _workAvailable.Release();


            // if the previous count is greater than zero, we have work waiting to be picked up
			if (count > 0 && _threads.Count < _maxThreads)
			{
				AddThread();
			}

            // build the thread pool up to minimum
            while (_threads.Count < _minThreads)
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
		}

		private void RunThread(object obj)
		{
			Thread currentThread = obj as Thread;

			WaitHandle[] handles = new WaitHandle[] {_shutdown, _workAvailable};

			int result;
			while ((result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(5), true)) != 0)
			{
				if (result == WaitHandle.WaitTimeout)
				{
					lock (_lockObj)
					{
						if (_threads.Count > _minThreads)
						{
							_threads.Remove(currentThread);
							break;
						}
					}

					continue;
				}


				T item;
				lock (_workItems)
					item = _workItems.Dequeue();

				try
				{
					_handler(item);
				}
				catch (Exception ex)
				{
					_log.Error("An exception occurred processing an item of type: " + typeof (T).FullName, ex);
				}
			}
		}
	}
}