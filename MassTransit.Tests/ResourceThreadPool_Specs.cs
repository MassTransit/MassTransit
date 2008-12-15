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
namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using Threading;

    [TestFixture]
    public class When_accessing_a_constrained_resource
    {
        private void Reader(SharedResource resource)
        {
            resource.DoSomething();
        }


        [Test]
        public void The_thread_pool_should_not_exceed_the_specified_limits()
        {
            SharedResource resource = new SharedResource {AvailableItems = 100};


            const int concurrentLimit = 5;
            const int maxThreads = 20;
            ResourceThreadPool<SharedResource, object> pool = new ResourceThreadPool<SharedResource, object>(resource, Reader, concurrentLimit, 1, maxThreads);
            try
            {
                resource.Completed.WaitOne(TimeSpan.FromSeconds(60), true);

                Assert.AreEqual(maxThreads, pool.CurrentThreadCount);
            }
            finally
            {
                pool.Dispose();
            }

            Assert.AreEqual(concurrentLimit, resource.MaxConcurrent);
        }
    }

    internal class SharedResource
    {
        private readonly ManualResetEvent _completed = new ManualResetEvent(false);
        private int _availableItems;
        private int _concurrent;
        private object _locker = new object();

        private int _maxConcurrent;

        public int AvailableItems
        {
            get { return _availableItems; }
            set { _availableItems = value; }
        }

        public WaitHandle Completed
        {
            get { return _completed; }
        }

        public int MaxConcurrent
        {
            get { return _maxConcurrent; }
        }

        public void DoSomething()
        {
            lock (_locker)
            {
                _concurrent++;
                _maxConcurrent = Math.Max(_concurrent, _maxConcurrent);
            }

            Thread.Sleep(100);

            lock (_locker)
                _concurrent--;
            if (Interlocked.Decrement(ref _availableItems) == 0)
                _completed.Set();
        }
    }
}