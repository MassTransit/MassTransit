namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Contracts;
    using Fabric;


    public class ClientNodeTopology
    {
        readonly Dictionary<ConsumerKey, long> _consumerMap;
        readonly Dictionary<long, TopologyEntry> _entries;
        readonly IMessageFabric _messageFabric;
        readonly IGrpcNode _node;
        long _lastSequenceNumber;
        Guid _sessionId;

        public ClientNodeTopology(IGrpcNode node, IMessageFabric messageFabric)
        {
            _node = node;
            _messageFabric = messageFabric;

            _entries = new Dictionary<long, TopologyEntry>();
            _consumerMap = new Dictionary<ConsumerKey, long>();
        }

        public void Join(Guid sessionId, IEnumerable<Topology> topologies)
        {
            if (sessionId != _sessionId)
            {
                Reset();
                _sessionId = sessionId;
            }

            foreach (var topology in topologies)
                ProcessTopology(topology);
        }

        public void ProcessTopology(Topology topology)
        {
            if (topology == null)
                throw new ArgumentNullException(nameof(topology));

            var topologySequenceNumber = topology.SequenceNumber;

            lock (_entries)
            {
                if (_entries.TryGetValue(topologySequenceNumber, out var existingEntry))
                {
                    if (existingEntry.Topology.ChangeCase != topology.ChangeCase)
                    {
                        LogContext.Warning?.Log("Topology Mismatch, expected {Existing} but was {New}", existingEntry.Topology.ChangeCase, topology
                            .ChangeCase);
                    }
                }
                else
                    _entries.Add(topologySequenceNumber, new TopologyEntry(topology));

                while (_entries.TryGetValue(_lastSequenceNumber + 1, out var nextEntry))
                {
                    if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.Exchange)
                    {
                        var exchangeName = nextEntry.Topology.Exchange.Name;

                        _messageFabric.ExchangeDeclare(_node, exchangeName, nextEntry.Topology.Exchange.Type);

                        _node.LogTopology(nextEntry.Topology.Exchange, nextEntry.Topology.Exchange.Type);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.Queue)
                    {
                        var queueName = nextEntry.Topology.Queue.Name;

                        _messageFabric.QueueDeclare(_node, queueName);

                        _node.LogTopology(nextEntry.Topology.Queue);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.ExchangeBind)
                    {
                        var source = nextEntry.Topology.ExchangeBind.Source;
                        var destination = nextEntry.Topology.ExchangeBind.Destination;

                        _messageFabric.ExchangeBind(_node, source, destination, nextEntry.Topology.ExchangeBind.RoutingKey);

                        _node.LogTopology(nextEntry.Topology.ExchangeBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.QueueBind)
                    {
                        var source = nextEntry.Topology.QueueBind.Source;
                        var destination = nextEntry.Topology.QueueBind.Destination;

                        _messageFabric.QueueBind(_node, source, destination);

                        _node.LogTopology(nextEntry.Topology.QueueBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.Consumer)
                    {
                        var consumer = nextEntry.Topology.Consumer;

                        var queueName = consumer.QueueName;

                        var queue = _messageFabric.GetQueue(_node, queueName);

                        var remoteConsumer = new GrpcRemoteConsumer(_node, queueName, consumer.ConsumerId);

                        nextEntry.Handle = queue.ConnectConsumer(_node, remoteConsumer);

                        var consumerKey = new ConsumerKey(queueName, consumer.ConsumerId);

                        _consumerMap[consumerKey] = nextEntry.Handle.Id;

                        _node.LogTopology(consumer);
                    }

                    _lastSequenceNumber = nextEntry.Topology.SequenceNumber;
                }
            }
        }

        public IEnumerable<Topology> GetTopology()
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
                _consumerMap.Clear();
            }
        }

        public long GetLocalConsumerId(string queueName, long consumerId)
        {
            return _consumerMap.TryGetValue(new ConsumerKey(queueName, consumerId), out var localConsumerId) ? localConsumerId : consumerId;
        }


        readonly struct ConsumerKey :
            IEquatable<ConsumerKey>
        {
            public bool Equals(ConsumerKey other)
            {
                return _queueName == other._queueName && _consumerId == other._consumerId;
            }

            public override bool Equals(object obj)
            {
                return obj is ConsumerKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_queueName.GetHashCode() * 397) ^ _consumerId.GetHashCode();
                }
            }

            public static bool operator ==(ConsumerKey left, ConsumerKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ConsumerKey left, ConsumerKey right)
            {
                return !left.Equals(right);
            }

            readonly string _queueName;
            readonly long _consumerId;

            public ConsumerKey(string queueName, long consumerId)
            {
                _queueName = queueName;
                _consumerId = consumerId;
            }
        }


        class TopologyEntry
        {
            public TopologyEntry(Topology topology)
            {
                Topology = topology;
            }

            public TopologyHandle Handle { get; set; }

            public Topology Topology { get; }

            public void Disconnect()
            {
                Handle?.Disconnect();

                Topology.Valid = false;
            }
        }
    }
}
