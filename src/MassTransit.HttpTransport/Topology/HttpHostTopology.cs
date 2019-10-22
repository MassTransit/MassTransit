namespace MassTransit.HttpTransport.Topology
{
    using Configuration;
    using MassTransit.Topology.Topologies;


    public class HttpHostTopology :
        HostTopology
    {
        public HttpHostTopology(IHttpTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
        }
    }
}
