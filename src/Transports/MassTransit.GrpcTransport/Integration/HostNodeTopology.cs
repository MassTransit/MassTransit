namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Fabric;


    public class HostNodeTopology
    {
        readonly Dictionary<long, TopologyEntry> _entries;
        long _nextSequenceNumber;

        public HostNodeTopology()
        {
            _entries = new Dictionary<long, TopologyEntry>();
        }

        public TopologyHandle Add(Topology topology, TopologyHandle handle)
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
            TopologyHandle
        {
            readonly TopologyHandle _handle;

            public TopologyEntry(Topology topology, TopologyHandle handle)
            {
                _handle = handle;

                Topology = topology;
            }

            public Topology Topology { get; }

            public long Id => _handle.Id;

            public void Disconnect()
            {
                _handle?.Disconnect();

                Topology.Valid = false;
            }
        }
    }
}
