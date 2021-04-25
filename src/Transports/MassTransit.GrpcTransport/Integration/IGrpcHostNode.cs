namespace MassTransit.GrpcTransport.Integration
{
    using System.Collections.Generic;
    using Contracts;
    using Fabric;


    public interface IGrpcHostNode :
        IGrpcNode
    {
        TopologyHandle AddTopology(Topology topology, TopologyHandle handle = default);

        IEnumerable<Topology> GetTopology();
    }
}
