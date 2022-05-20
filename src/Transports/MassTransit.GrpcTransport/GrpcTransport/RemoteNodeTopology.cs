namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fabric;
    using Transports.Fabric;


    public class RemoteNodeTopology
    {
        readonly Dictionary<long, TopologyEntry> _entries;
        readonly IMessageFabric<NodeContext, GrpcTransportMessage> _messageFabric;
        readonly IGrpcNode _node;
        readonly Dictionary<ReceiverKey, long> _receiverMap;
        long _lastSequenceNumber;
        Guid _sessionId;

        public RemoteNodeTopology(IGrpcNode node, IMessageFabric<NodeContext, GrpcTransportMessage> messageFabric)
        {
            _node = node;
            _messageFabric = messageFabric;

            _entries = new Dictionary<long, TopologyEntry>();
            _receiverMap = new Dictionary<ReceiverKey, long>();
        }

        public void Join(Guid sessionId, IEnumerable<Contracts.Topology> topologies)
        {
            if (sessionId != _sessionId)
            {
                Reset();
                _sessionId = sessionId;
            }

            foreach (var topology in topologies)
                ProcessTopology(topology);
        }

        public void ProcessTopology(Contracts.Topology topology)
        {
            if (topology == null)
                throw new ArgumentNullException(nameof(topology));

            var topologySequenceNumber = topology.SequenceNumber;

            lock (_entries)
            {
                if (_entries.TryGetValue(topologySequenceNumber, out var existingEntry))
                {
                    if (existingEntry.Topology.ChangeCase != topology.ChangeCase)
                        LogContext.Warning?.Log("Topology Mismatch, {Existing} != {New}", existingEntry.Topology.ChangeCase, topology.ChangeCase);
                }
                else
                    _entries.Add(topologySequenceNumber, new TopologyEntry(topology));

                while (_entries.TryGetValue(_lastSequenceNumber + 1, out var nextEntry))
                {
                    if (nextEntry.Topology.ChangeCase == Contracts.Topology.ChangeOneofCase.Exchange)
                    {
                        var exchange = nextEntry.Topology.Exchange;

                        _messageFabric.ExchangeDeclare(_node, exchange.Name, exchange.Type.ToExchangeType());

                        _node.LogTopology(exchange, exchange.Type);
                    }
                    else if (nextEntry.Topology.ChangeCase == Contracts.Topology.ChangeOneofCase.Queue)
                    {
                        var queue = nextEntry.Topology.Queue;

                        _messageFabric.QueueDeclare(_node, queue.Name);

                        _node.LogTopology(queue);
                    }
                    else if (nextEntry.Topology.ChangeCase == Contracts.Topology.ChangeOneofCase.ExchangeBind)
                    {
                        var exchangeBind = nextEntry.Topology.ExchangeBind;

                        _messageFabric.ExchangeBind(_node, exchangeBind.Source, exchangeBind.Destination, exchangeBind.RoutingKey);

                        _node.LogTopology(exchangeBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Contracts.Topology.ChangeOneofCase.QueueBind)
                    {
                        var queueBind = nextEntry.Topology.QueueBind;

                        _messageFabric.QueueBind(_node, queueBind.Source, queueBind.Destination);

                        _node.LogTopology(queueBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Contracts.Topology.ChangeOneofCase.Receiver)
                    {
                        var receiver = nextEntry.Topology.Receiver;
                        var queueName = receiver.QueueName;

                        var queue = _messageFabric.GetQueue(_node, queueName);

                        var messageReceiver = new RemoteNodeMessageReceiver(_node, queueName, receiver.ReceiverId);

                        nextEntry.Handle = queue.ConnectMessageReceiver(_node, messageReceiver);

                        var key = new ReceiverKey(queueName, receiver.ReceiverId);

                        _receiverMap[key] = nextEntry.Handle.Id;

                        _node.LogTopology(receiver);
                    }

                    _lastSequenceNumber = nextEntry.Topology.SequenceNumber;
                }
            }
        }

        public IEnumerable<Contracts.Topology> GetTopology()
        {
            lock (_entries)
                return _entries.Values.Where(x => x.Topology.SequenceNumber <= _lastSequenceNumber).Select(x => x.Topology).ToList();
        }

        void Reset()
        {
            lock (_entries)
            {
                foreach (var entry in _entries.Values)
                    entry.Disconnect();

                _lastSequenceNumber = 0;

                _entries.Clear();
                _receiverMap.Clear();
            }
        }

        public long GetLocalConsumerId(string queueName, long consumerId)
        {
            return _receiverMap.TryGetValue(new ReceiverKey(queueName, consumerId), out var localConsumerId) ? localConsumerId : consumerId;
        }


        readonly struct ReceiverKey :
            IEquatable<ReceiverKey>
        {
            public bool Equals(ReceiverKey other)
            {
                return _queueName == other._queueName && _consumerId == other._consumerId;
            }

            public override bool Equals(object obj)
            {
                return obj is ReceiverKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_queueName.GetHashCode() * 397) ^ _consumerId.GetHashCode();
                }
            }

            public static bool operator ==(ReceiverKey left, ReceiverKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ReceiverKey left, ReceiverKey right)
            {
                return !left.Equals(right);
            }

            readonly string _queueName;
            readonly long _consumerId;

            public ReceiverKey(string queueName, long consumerId)
            {
                _queueName = queueName;
                _consumerId = consumerId;
            }
        }


        class TopologyEntry
        {
            public TopologyEntry(Contracts.Topology topology)
            {
                Topology = topology;
            }

            public TopologyHandle Handle { get; set; }

            public Contracts.Topology Topology { get; }

            public void Disconnect()
            {
                Handle?.Disconnect();

                Topology.Valid = false;
            }
        }
    }
}
