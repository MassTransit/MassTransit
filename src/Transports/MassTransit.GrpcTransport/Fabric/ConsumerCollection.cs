namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Util;


    public class ConsumerCollection
    {
        readonly LoadBalancerFactory _balancerFactory;
        readonly Dictionary<long, IGrpcQueueConsumer> _connections;
        TaskCompletionSource<IConsumerLoadBalancer> _balancer;
        IGrpcQueueConsumer[] _connected;
        long _nextId;

        public ConsumerCollection(LoadBalancerFactory balancerFactory)
        {
            _balancerFactory = balancerFactory;

            _balancer = TaskUtil.GetTask<IConsumerLoadBalancer>();
            _connections = new Dictionary<long, IGrpcQueueConsumer>();
            _connected = new IGrpcQueueConsumer[0];
        }

        /// <summary>
        /// The number of consumer
        /// </summary>
        public int Count => _connected.Length;

        /// <summary>
        /// Connect the consumer to the queue
        /// </summary>
        /// <param name="consumer">The connection to add</param>
        /// <returns>The connection handle</returns>
        public TopologyHandle Connect(IGrpcQueueConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));

            lock (_connections)
            {
                var id = ++_nextId;

                _connections.Add(id, consumer);
                _connected = _connections.Values.ToArray();

                var balancer = _connected.Length == 1
                    ? new SingleConsumerLoadBalancer(_connected[0])
                    : _balancerFactory(_connected);

                if (!_balancer.TrySetResult(balancer))
                {
                    _balancer = TaskUtil.GetTask<IConsumerLoadBalancer>();
                    _balancer.SetResult(balancer);
                }

                return new Handle(id, this);
            }
        }

        /// <summary>
        /// Enumerate the connections invoking the callback for each connection
        /// </summary>
        /// <param name="callback">The callback</param>
        /// <returns>An awaitable Task for the operation</returns>
        public Task ForEachAsync(Func<IGrpcQueueConsumer, Task> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            IGrpcQueueConsumer[] connected;
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

        public Task<IGrpcQueueConsumer> Next(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            Task<IConsumerLoadBalancer> task = _balancer.Task;
            if (task.IsCompletedSuccessfully())
            {
                var balancer = task.GetAwaiter().GetResult();
                var consumer = balancer.SelectConsumer(message);

                return Task.FromResult(consumer);
            }

            async Task<IGrpcQueueConsumer> NextAsync()
            {
                var balancer = await _balancer.Task.OrCanceled(cancellationToken).ConfigureAwait(false);

                return balancer.SelectConsumer(message);
            }

            return NextAsync();
        }

        public bool All(Func<IGrpcQueueConsumer, bool> callback)
        {
            IGrpcQueueConsumer[] connected;
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

                _balancer = TaskUtil.GetTask<IConsumerLoadBalancer>();

                if (_connected.Length > 0)
                {
                    var balancer = _connected.Length == 1
                        ? new SingleConsumerLoadBalancer(_connected[0])
                        : _balancerFactory(_connected);

                    _balancer.SetResult(balancer);
                }
            }
        }

        public bool TryGetConsumer(long id, out IGrpcQueueConsumer consumer)
        {
            lock (_connections)
                return _connections.TryGetValue(id, out consumer);
        }


        class Handle :
            TopologyHandle
        {
            readonly ConsumerCollection _connectable;

            public Handle(long id, ConsumerCollection connectable)
            {
                Id = id;
                _connectable = connectable;
            }

            public long Id { get; }

            public void Disconnect()
            {
                _connectable.Disconnect(Id);
            }
        }
    }
}
