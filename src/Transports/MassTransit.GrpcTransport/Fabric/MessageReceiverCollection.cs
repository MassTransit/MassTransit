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


    public class MessageReceiverCollection :
        IProbeSite
    {
        readonly LoadBalancerFactory _balancerFactory;
        readonly Dictionary<long, IMessageReceiver> _receivers;
        TaskCompletionSource<IReceiverLoadBalancer> _balancer;
        long _nextId;

        public MessageReceiverCollection(LoadBalancerFactory balancerFactory)
        {
            _balancerFactory = balancerFactory;

            _balancer = TaskUtil.GetTask<IReceiverLoadBalancer>();
            _receivers = new Dictionary<long, IMessageReceiver>();
        }

        public void Probe(ProbeContext context)
        {
            IMessageReceiver[] connected;
            lock (_receivers)
                connected = _receivers.Values.ToArray();

            if (connected.Length == 0)
                return;

            var scope = context.CreateScope("receiver");

            for (var i = 0; i < connected.Length; i++)
                connected[i].Probe(scope);
        }

        public TopologyHandle Connect(IMessageReceiver receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            lock (_receivers)
            {
                var id = ++_nextId;

                _receivers.Add(id, receiver);

                IMessageReceiver[] connected = _receivers.Values.ToArray();

                var balancer = connected.Length == 1
                    ? new SingleReceiverLoadBalancer(connected[0])
                    : _balancerFactory(connected);

                if (!_balancer.TrySetResult(balancer))
                {
                    _balancer = TaskUtil.GetTask<IReceiverLoadBalancer>();
                    _balancer.SetResult(balancer);
                }

                return new Handle(id, this);
            }
        }

        public Task<IMessageReceiver> Next(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            Task<IReceiverLoadBalancer> task = _balancer.Task;
            if (task.IsCompletedSuccessfully())
            {
                var balancer = task.GetAwaiter().GetResult();
                var consumer = balancer.SelectReceiver(message);

                return Task.FromResult(consumer);
            }

            async Task<IMessageReceiver> NextAsync()
            {
                var balancer = await _balancer.Task.OrCanceled(cancellationToken).ConfigureAwait(false);

                return balancer.SelectReceiver(message);
            }

            return NextAsync();
        }

        public bool TryGetReceiver(long id, out IMessageReceiver consumer)
        {
            lock (_receivers)
                return _receivers.TryGetValue(id, out consumer);
        }

        void Disconnect(long id)
        {
            lock (_receivers)
            {
                _receivers.Remove(id);

                _balancer = TaskUtil.GetTask<IReceiverLoadBalancer>();

                IMessageReceiver[] connected = _receivers.Values.ToArray();
                if (connected.Length <= 0)
                    return;

                var balancer = connected.Length == 1
                    ? new SingleReceiverLoadBalancer(connected[0])
                    : _balancerFactory(connected);

                _balancer.SetResult(balancer);
            }
        }


        class Handle :
            TopologyHandle
        {
            readonly MessageReceiverCollection _connectable;

            public Handle(long id, MessageReceiverCollection connectable)
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
