namespace MassTransit.GrpcTransport.Integration
{
    using System.Collections.Generic;
    using Contexts;
    using Contracts;
    using Fabric;


    public sealed class GrpcHostNode :
        GrpcNode,
        IGrpcHostNode
    {
        readonly HostNodeTopology _hostTopology;

        public GrpcHostNode(IMessageFabric messageFabric, NodeContext context)
            : base(messageFabric, context)
        {
            _hostTopology = new HostNodeTopology();

            SetReady();
        }

        public TopologyHandle AddTopology(Topology topology, TopologyHandle handle)
        {
            return _hostTopology.Add(topology, handle);
        }

        public IEnumerable<Topology> GetTopology()
        {
            return _hostTopology.GetTopology();
        }
    }
}
