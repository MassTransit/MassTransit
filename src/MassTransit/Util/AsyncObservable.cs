// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;


    /// <summary>
    /// Supports the asynchronous notification of events to an observer that are maintained in order
    /// to prevent issues with event ordering or threading.
    /// </summary>
    /// <typeparam name="T">The observer type</typeparam>
    public class AsyncObservable<T>
        where T : class
    {
        public delegate Task ObserverNotification(T observer);


        readonly ConcurrentDictionary<long, Handle> _connections;
        long _nextId;

        public AsyncObservable()
        {
            _connections = new ConcurrentDictionary<long, Handle>();
        }

        /// <summary>
        /// The number of connections
        /// </summary>
        public int Count
        {
            get { return _connections.Count; }
        }

        /// <summary>
        /// Connect a connectable type
        /// </summary>
        /// <param name="observer">The connection to add</param>
        /// <returns>The connection handle</returns>
        public ConnectHandle Connect(T observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            long id = Interlocked.Increment(ref _nextId);

            var handle = new Handle(id, observer, Disconnect);

            bool added = _connections.TryAdd(id, handle);
            if (!added)
                throw new InvalidOperationException("The observer could not be added");

            return handle;
        }

        /// <summary>
        /// Enumerate the connections invoking the callback for each connection
        /// </summary>
        /// <param name="callback">The callback</param>
        /// <returns>An awaitable Task for the operation</returns>
        public void Notify(ObserverNotification callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (_connections.Count == 0)
                return;

            foreach (Handle observer in _connections.Values)
                observer.Notify(callback);
        }

        void Disconnect(long id)
        {
            Handle ignored;
            _connections.TryRemove(id, out ignored);
        }


        class Handle :
            ConnectHandle
        {
            static readonly ILog _log = Logger.Get<AsyncObservable<T>>();
            readonly CancellationTokenSource _cancellation;
            readonly Action<long> _disconnect;
            readonly long _id;
            readonly Task _notifyTask;
            readonly T _observer;
            readonly QueuedTaskScheduler _scheduler;

            public Handle(long id, T observer, Action<long> disconnect)
            {
                _id = id;
                _disconnect = disconnect;
                _observer = observer;

                _cancellation = new CancellationTokenSource();

                _scheduler = new QueuedTaskScheduler(TaskScheduler.Default, 1);
            }

            public void Disconnect()
            {
                _disconnect(_id);
            }

            void IDisposable.Dispose()
            {
                Disconnect();

                _scheduler.Dispose();
            }

            public void Notify(ObserverNotification notification)
            {
                Task.Factory.StartNew(() => notification(_observer),
                    _cancellation.Token, TaskCreationOptions.HideScheduler, _scheduler);
            }
        }
    }
}