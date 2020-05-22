namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using Configuration;
    using Context;
    using GreenPipes;
    using Topology;


    public class ServiceBusEntityReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ServiceBusReceiveEndpointContext
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusEntityReceiveEndpointContext(IServiceBusHostConfiguration hostConfiguration, IServiceBusEntityEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;

            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        public IRetryPolicy RetryPolicy => _hostConfiguration.RetryPolicy;

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }
    }
}
