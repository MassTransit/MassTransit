namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;


    public class InMemoryReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IInMemoryReceiveEndpointConfiguration _configuration;
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryReceiveEndpointBuilder(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                IInMemoryMessageConsumeTopologyConfigurator<T> topology = _configuration.Topology.Consume.GetMessageTopology<T>();
                if (topology.ConfigureConsumeTopology)
                    topology.Bind();
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public InMemoryReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new TransportInMemoryReceiveEndpointContext(_hostConfiguration, _configuration);

            context.GetOrAddPayload(() => _hostConfiguration.Topology);

            return context;
        }
    }
}
