namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using GreenPipes;


    public class HostNodeTopology
    {
        readonly Dictionary<long, TopologyEntry> _entries;
        long _nextSequenceNumber;

        public HostNodeTopology()
        {
            _entries = new Dictionary<long, TopologyEntry>();
        }

        public ConnectHandle Add(Topology topology, ConnectHandle handle)
        {
            if (topology == null)
                throw new ArgumentNullException(nameof(topology));

            lock (_entries)
            {
                var sequenceNumber = ++_nextSequenceNumber;

                topology.SequenceNumber = sequenceNumber;

                var entry = new TopologyEntry(topology, handle);
                _entries.Add(sequenceNumber, entry);

                return entry;
            }
        }

        public IEnumerable<Topology> GetTopology()
        {
            lock (_entries)
                return _entries.Values.Select(x => x.Topology).ToList();
        }


        class TopologyEntry :
            ConnectHandle
        {
            readonly ConnectHandle _handle;

            public TopologyEntry(Topology topology, ConnectHandle handle)
            {
                _handle = handle;

                Topology = topology;
            }

            public Topology Topology { get; }

            public void Disconnect()
            {
                _handle?.Disconnect();

                Topology.Valid = false;
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}
