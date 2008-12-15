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
	using System.Transactions;
	using log4net;
	using Util;

	public class ResourceThreadPool<TResource, TElement> :
		IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ResourceThreadPool<TResource, TElement>));

		private readonly Action<TResource> _action;
		private readonly TResource _resource;
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly List<Thread> _threads = new List<Thread>();
		private bool _disposed;
		private int _maxThreads;
		private int _minThreads;
		private Semaphore _resourceGovernor;
		private int _resourceLimit;

		public ResourceThreadPool(TResource resource,
		                          Action<TResource> action,
		                          int resourceLimit,
		                          int minThreads,
		                          int maxThreads)
		{
			Check.Ensure(resourceLimit > 0, "The resource limit must be greater than zero");
			Check.Ensure(minThreads > 0, "The minimum thread count must be greater than zero");
			Check.Ensure(maxThreads >= minThreads, string.Format("The maximum thread count (currently={0}) must be at least equal to the minimum thread count (currently={1})", maxThreads, minThreads));
			Check.Parameter(action).WithMessage("The handler must not be null").IsNotNull();

			_resource = resource;
			_action = action;
			_minThreads = minThreads;
			_maxThreads = maxThreads;
			_resourceLimit = resourceLimit;
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
				Check.Ensure(value > 0, "The minimum thread count must be greater than zero");
				Check.Ensure(_maxThreads >= value, "The maximum thread count must be at least equal to the minimum thread count");

				_minThreads = value;
			}
		}

		public int CurrentThreadCount
		{
			[DebuggerStepThrough]
			get { lock (_threads) return _threads.Count; }
		}

		public int ResourceLimit
		{
			[DebuggerStepThrough]
			get { return _resourceLimit; }
			set
			{
				Check.Ensure(value > 0, "The resource limit must be greather than zero");
				Check.Ensure(_maxThreads >= value, "The resource limit must be less than or equal to the number of threads");
				_resourceLimit = value;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ResourceThreadPool()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
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
			_disposed = true;
		}

		private void AdjustQueueCount()
		{
			if (CurrentThreadCount < MaxThreads)
			{
				bool available = _resourceGovernor.WaitOne(0, false);
				try
				{
					if (available)
						AddThread();
				}
				finally
				{
					if (available)
						_resourceGovernor.Release(1);
				}
			}

			while (CurrentThreadCount < MinThreads)
				AddThread();
		}

		private void AddThread()
		{
			Thread thread = new Thread(RunThread);
			thread.SetApartmentState(ApartmentState.MTA);

			lock (_threads)
				_threads.Add(thread);

			thread.Start();

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Created Thread {0}, Total Threads = {1}", thread.ManagedThreadId, CurrentThreadCount);
		}

		private void RunThread()
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting Thread {0}", threadId);

			WaitHandle[] handles = new WaitHandle[] {_shutdown, _resourceGovernor};

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
					_action(_resource);
				}
				catch (Exception ex)
				{
					_log.Error("An exception occurred processing an item of type: " + typeof (TResource).FullName, ex);
				}
			}

			AdjustQueueCount();

			if (_log.IsDebugEnabled)
				_log.DebugFormat("ExitingThread {0}", threadId);
		}

		public void WakeUp()
		{
			if (_resourceGovernor == null)
			{
				_resourceGovernor = new Semaphore(_resourceLimit, _resourceLimit);
			}

			AdjustQueueCount();
		}

		public void ReleaseResource(int releaseCount)
		{
			_resourceGovernor.Release(releaseCount);

			AdjustQueueCount();
		}
	}
}