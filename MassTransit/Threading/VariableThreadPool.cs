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
namespace MassTransit.Threading
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using log4net;
	using Util;

	/// <summary>
	/// A variable size pool of threads
	/// </summary>
	public class DynamicThreadPool :
		IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (DynamicThreadPool));

		private readonly Func<bool> _handler;
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly Dictionary<int, Thread> _threads = new Dictionary<int, Thread>();
		private volatile bool _disposed;
		private int _maxThreads;
		private int _minThreads;
		private volatile bool _running;

		public DynamicThreadPool(Func<bool> handler, int minThreads, int maxThreads)
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
			[DebuggerStepThrough]
			get { return _maxThreads; }
			set
			{
				Check.Ensure(value >= _minThreads, "The maximum thread count must be at least equal to the minimum thread count");

				_maxThreads = value;
			}
		}

		public int MinThreads
		{
			[DebuggerStepThrough]
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
			[DebuggerStepThrough]
			get { lock (_threads) return _threads.Count; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (_running)
				return;

			for (int i = 0; i < _minThreads; i++)
			{
				AddThread();
			}

			_running = true;
		}

		public void Stop()
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (!_running)
				return;

			Thread[] threads;
			lock (_threads)
			{
				threads = new Thread[_threads.Count];

				_threads.Values.CopyTo(threads, 0);
			}

			_shutdown.Set();

			for (int i = 0; i < threads.Length; i++)
			{
				if (!threads[i].Join(TimeSpan.FromSeconds(60)))
				{
					_log.WarnFormat("Thread {0} did not exit within the timeout period", threads[i].ManagedThreadId);
				}
			}

			_threads.Clear();
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Stop();
			}
			_disposed = true;
		}

		private void AddThread()
		{
			lock (_threads)
			{
				if (_threads.Count >= _maxThreads)
					return;
			}

			Thread thread = new Thread(RunThread);

			lock (_threads)
			{
				thread.Start();

				_threads[thread.ManagedThreadId] = thread;
			}

			if (_log.IsInfoEnabled)
				_log.InfoFormat("Created Thread {0}, Total Threads = {1}", thread.ManagedThreadId, CurrentThreadCount);
		}

		private void RunThread()
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;
			try
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Started Thread {0}", threadId);

				int workload = 0;
				while (_shutdown.WaitOne(0, false) == false)
				{
					try
					{
						bool keepWorking = _handler();

						workload = keepWorking ? Math.Max(1, workload + 1) : Math.Min(0, workload - 1);
						if(workload > 10)
						{
							AddThread();
							workload = 0;
						}
						else if (workload < -10)
						{
							// exit this thread if we didn't do any work and we're above the minimum thread count
							lock (_threads)
								if (_threads.Count > _minThreads)
									break;
						}
					}
					catch (Exception ex)
					{
						_log.Error("The handler through an exception", ex);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("An exception occurred within thread " + threadId, ex);
			}
			finally
			{
				lock (_threads)
					_threads.Remove(threadId);
			}

			if (_log.IsInfoEnabled)
				_log.InfoFormat("ExitingThread {0}", threadId);
		}

		~DynamicThreadPool()
		{
			Dispose(false);
		}
	}
}