namespace MassTransit.InMemoryTransport
{
    using Configuration;
    using Topology;
    using Transports;


    public class InMemoryBusTopology :
        BusTopology,
        IInMemoryBusTopology
    {
        readonly IInMemoryTopologyConfiguration _configuration;

        public InMemoryBusTopology(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration configuration)
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
