namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;
    using Fabric;


    public interface IGrpcHostNode :
        IGrpcNode
    {
        TopologyHandle AddTopology(Contracts.Topology topology, TopologyHandle handle = default);

        IEnumerable<Contracts.Topology> GetTopology();
    }
}
