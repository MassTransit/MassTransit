namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Util;


    public class ConsumerCollection<T>
        where T : class
    {
        readonly Dictionary<long, T> _connections;
        T[] _connected;
        long _index;
        long _nextId;

        public ConsumerCollection()
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

            var id = Interlocked.Increment(ref _nextId);

            lock (_connections)
            {
                _connections.Add(id, connection);
                _connected = _connections.Values.ToArray();
            }

            return new Handle(id, this);
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
                connected = _connected;

            if (connected.Length == 0)
                return TaskUtil.Completed;

            if (connected.Length == 1)
                return callback(connected[0]);

            var outputTasks = new Task[connected.Length];
            int i;
            for (i = 0; i < connected.Length; i++)
                outputTasks[i] = callback(connected[i]);

            for (i = 0; i < outputTasks.Length; i++)
            {
                if (!outputTasks[i].IsCompletedSuccessfully())
                    break;
            }

            if (i == outputTasks.Length)
                return TaskUtil.Completed;

            return Task.WhenAll(outputTasks);
        }

        public T Next()
        {
            T[] connected;
            lock (_connections)
                connected = _connected;

            if (connected.Length == 0)
                return default;

            var next = _index++ % connected.Length;

            return connected[next];
        }

        public bool All(Func<T, bool> callback)
        {
            T[] connected;
            lock (_connections)
                connected = _connected;

            if (connected.Length == 0)
                return true;

            if (connected.Length == 1)
                return callback(connected[0]);

            for (var i = 0; i < connected.Length; i++)
            {
                if (callback(connected[i]) == false)
                    return false;
            }

            return true;
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
            readonly ConsumerCollection<T> _connectable;
            readonly long _id;

            public Handle(long id, ConsumerCollection<T> connectable)
            {
                _id = id;
                _connectable = connectable;
            }

            public void Disconnect()
            {
                _connectable.Disconnect(_id);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}
