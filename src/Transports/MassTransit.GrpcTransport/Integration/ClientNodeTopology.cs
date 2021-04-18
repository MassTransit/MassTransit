namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Contracts;
    using Fabric;
    using GreenPipes;


    public class ClientNodeTopology
    {
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

                        _messageFabric.ExchangeDeclare(_node, exchangeName);

                        _node.LogTopology(nextEntry.Topology.Exchange);
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

                        _messageFabric.ExchangeBind(_node, source, destination, nextEntry.Topology.ExchangeBind.RoutingKey.ToStringValue());

                        _node.LogTopology(nextEntry.Topology.ExchangeBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.QueueBind)
                    {
                        var source = nextEntry.Topology.QueueBind.Source;
                        var destination = nextEntry.Topology.QueueBind.Destination;

                        _messageFabric.QueueBind(_node, source, destination, nextEntry.Topology.QueueBind.RoutingKey.ToStringValue());

                        _node.LogTopology(nextEntry.Topology.QueueBind);
                    }
                    else if (nextEntry.Topology.ChangeCase == Topology.ChangeOneofCase.Consumer)
                    {
                        var queueName = nextEntry.Topology.Consumer.QueueName;

                        var queue = _messageFabric.GetQueue(_node, queueName);

                        var consumer = new GrpcRemoteConsumer(_node);

                        nextEntry.Handle = queue.ConnectConsumer(_node, consumer);

                        _node.LogTopology(nextEntry.Topology.Consumer);
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
            }
        }


        class TopologyEntry
        {
            public TopologyEntry(Topology topology)
            {
                Topology = topology;
            }

            public ConnectHandle Handle { get; set; }

            public Topology Topology { get; }

            public void Disconnect()
            {
                Handle?.Disconnect();

                Topology.Valid = false;
            }
        }
    }
}
