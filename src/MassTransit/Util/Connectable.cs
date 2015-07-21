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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Maintains a collection of connections of the generic type
    /// </summary>
    /// <typeparam name="T">The connectable type</typeparam>
    public class Connectable<T>
        where T : class
    {
        readonly Dictionary<long, T> _connections;
        T[] _connected;
        long _nextId;

        public Connectable()
        {
            _connections = new Dictionary<long, T>();
            _connected = new T[0];
        }

        /// <summary>
        /// The number of connections
        /// </summary>
        public int Count => _connected.Length;

        /// <summary>
        /// Connect a connectable type
        /// </summary>
        /// <param name="connection">The connection to add</param>
        /// <returns>The connection handle</returns>
        public ConnectHandle Connect(T connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            long id = Interlocked.Increment(ref _nextId);

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
        public Task ForEachAsync(Func<T, Task> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            T[] connected;
            lock (_connections)
            {
                connected = _connected;
            }

            if (connected.Length == 0)
                return TaskUtil.Completed;

            if (connected.Length == 1)
                return callback(_connections.First().Value);

            return Task.WhenAll(connected.Select(callback));
        }

        public bool All(Func<T, bool> callback)
        {
            T[] connected;
            lock (_connections)
            {
                connected = _connected;
            }
            return connected.All(callback);
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