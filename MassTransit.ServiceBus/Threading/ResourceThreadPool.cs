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
namespace MassTransit.ServiceBus.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Transactions;
    using log4net;
    using Util;

    public delegate T Function<R, T>(R resource);

    public class ResourceThreadPool<TResource, TElement> :
        IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ResourceThreadPool<TResource, TElement>));

        private readonly Action<TElement> _action;
        private readonly Function<TResource, TElement> _function;
        private readonly TResource _resource;
        private readonly Semaphore _resourceGovernor;
        private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
        private readonly List<Thread> _threads = new List<Thread>();
        private int _maxThreads;
        private int _minThreads;

        public ResourceThreadPool(TResource resource, Function<TResource, TElement> function, Action<TElement> action)
            : this(resource, function, action, 1, 1, 10)
        {
        }

        public ResourceThreadPool(TResource resource, Function<TResource, TElement> function, Action<TElement> action, int resourceLimit)
            : this(resource, function, action, resourceLimit, 1, 10)
        {
        }

        public ResourceThreadPool(TResource resource, Function<TResource, TElement> function, Action<TElement> action, int resourceLimit, int minThreads, int maxThreads)
        {
            Check.Ensure(resourceLimit > 0, "The resource limit must be greater than zero");
            Check.Ensure(minThreads > 0, "The minimum thread count must be greater than zero");
            Check.Ensure(maxThreads >= minThreads, "The maximum thread count must be at least equal to the minimum thread count");
            Check.Parameter(action).WithMessage("The handler must not be null").IsNotNull();

            _resource = resource;
            _function = function;
            _action = action;
            _resourceGovernor = new Semaphore(resourceLimit, resourceLimit);
            _minThreads = minThreads;
            _maxThreads = maxThreads;

            AdjustQueueCount();
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
					using (TransactionScope scope = new TransactionScope())
					{
						TElement element;
						try
						{
							AdjustQueueCount();

							element = _function(_resource);
						}
						finally
						{
							_resourceGovernor.Release(1);
						}

						_action(element);

						scope.Complete();
					}
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
    }
}