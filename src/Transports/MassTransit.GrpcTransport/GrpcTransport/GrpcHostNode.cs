namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;
    using Fabric;
    using Transports.Fabric;


    public sealed class GrpcHostNode :
        GrpcNode,
        IGrpcHostNode
    {
        readonly HostNodeTopology _hostTopology;

        public GrpcHostNode(IMessageFabric<NodeContext, GrpcTransportMessage> messageFabric, NodeContext context)
            : base(messageFabric, context)
        {
            _hostTopology = new HostNodeTopology();

            SetReady();
        }

        public TopologyHandle AddTopology(Contracts.Topology topology, TopologyHandle handle)
        {
            return _hostTopology.Add(topology, handle);
        }

        public IEnumerable<Contracts.Topology> GetTopology()
        {
            return _hostTopology.GetTopology();
        }
    }
}
