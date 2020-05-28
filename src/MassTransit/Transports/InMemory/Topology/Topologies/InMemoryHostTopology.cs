namespace MassTransit.Transports.InMemory.Topology.Topologies
{
    using Configuration;
    using MassTransit.Topology.Topologies;


    public class InMemoryHostTopology :
        HostTopology,
        IInMemoryHostTopology
    {
        readonly IInMemoryTopologyConfiguration _configuration;

        public InMemoryHostTopology(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _configuration = configuration;
        }

        public new IInMemoryMessagePublishTopology<T> Publish<T>()
            where T : class
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }
    }
}
