// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using GreenPipes;


    class CacheValueObservable<TValue> : ICacheValueObserver<TValue>
        where TValue : class
    {
        readonly Dictionary<long, ICacheValueObserver<TValue>> _connections;
        ICacheValueObserver<TValue>[] _connected;
        long _nextId;

        public CacheValueObservable()
        {
            _connections = new Dictionary<long, ICacheValueObserver<TValue>>();
            _connected = new ICacheValueObserver<TValue>[0];
        }

        public void ValueAdded(INode<TValue> node, TValue value)
        {
            ForEach(x => x.ValueAdded(node, value));
        }

        public void ValueRemoved(INode<TValue> node, TValue value)
        {
            ForEach(x => x.ValueRemoved(node, value));
        }

        public void CacheCleared()
        {
            ForEach(x => x.CacheCleared());
        }

        /// <summary>
        /// Connect a connectable type
        /// </summary>
        /// <param name="connection">The connection to add</param>
        /// <returns>The connection handle</returns>
        public ConnectHandle Connect(ICacheValueObserver<TValue> connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var id = Interlocked.Increment(ref _nextId);

            lock (_connections)
            {
                _connections.Add(id, connection);
                _connected = _connections.Values.ToArray();
            }

            return new Handle(id, Disconnect);
        }

        /// <summary>
        /// Enumerate the connections invoking the callback for each connection
        /// </summary>
        /// <param name="callback">The callback</param>
        /// <returns>An awaitable Task for the operation</returns>
        public void ForEach(Action<ICacheValueObserver<TValue>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            ICacheValueObserver<TValue>[] connected;
            lock (_connections)
            {
                if (_connected.Length == 0)
                    return;

                connected = _connected;
            }

            if (connected.Length == 1)
            {
                callback(connected[0]);
            }
            else
            {
                for (var i = 0; i < connected.Length; i++)
                {
                    callback(connected[i]);
                }
            }
        }

        void Disconnect(long id)
        {
            lock (_connections)
            {
                _connections.Remove(id);
                _connected = _connections.Values.ToArray();
            }
        }


        class Handle :
            ConnectHandle
        {
            readonly Action<long> _disconnect;
            readonly long _id;

            public Handle(long id, Action<long> disconnect)
            {
                _id = id;
                _disconnect = disconnect;
            }

            public void Disconnect()
            {
                _disconnect(_id);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}