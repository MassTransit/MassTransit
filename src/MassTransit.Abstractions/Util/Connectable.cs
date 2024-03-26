namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
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
        T[]? _connected;
        long _nextId;

        public Connectable()
        {
            _connections = new Dictionary<long, T>();
            _connected = null;
        }

        public T[] Connected
        {
            get
            {
                T[]? read = Volatile.Read(ref _connected);
                if (read != null)
                    return read;

                lock (_connections)
                {
                    read = Volatile.Read(ref _connected);
                    if (read != null)
                        return read;

                    var connected = new T[_connections.Count];
                    _connections.Values.CopyTo(connected, 0);

                    Volatile.Write(ref _connected, connected);

                    return connected;
                }
            }
        }

        /// <summary>
        /// The number of connections
        /// </summary>
        public int Count => Connected.Length;

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
                _connected = null;
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

            T[] connected = Connected;

            if (connected.Length == 0)
                return Task.CompletedTask;

            if (connected.Length == 1)
                return callback(connected[0]);

            var outputTasks = new Task[connected.Length];
            int i;
            for (i = 0; i < connected.Length; i++)
                outputTasks[i] = callback(connected[i]);

            for (i = 0; i < outputTasks.Length; i++)
            {
                if (outputTasks[i].Status != TaskStatus.RanToCompletion)
                    break;
            }

            if (i == outputTasks.Length)
                return Task.CompletedTask;

            return Task.WhenAll(outputTasks);
        }

        public void ForEach(Action<T> callback)
        {
            T[] connected = Connected;

            switch (connected.Length)
            {
                case 0:
                    break;
                case 1:
                    callback(connected[0]);
                    break;
                default:
                {
                    for (var i = 0; i < connected.Length; i++)
                        callback(connected[i]);
                    break;
                }
            }
        }

        public bool All(Func<T, bool> callback)
        {
            T[] connected = Connected;

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
                _connected = null;
            }
        }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }


        class Handle :
            ConnectHandle
        {
            readonly Connectable<T> _connectable;
            readonly long _id;

            public Handle(long id, Connectable<T> connectable)
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

            public void Method1()
            {
            }

            public void Method2()
            {
            }
        }
    }
}
