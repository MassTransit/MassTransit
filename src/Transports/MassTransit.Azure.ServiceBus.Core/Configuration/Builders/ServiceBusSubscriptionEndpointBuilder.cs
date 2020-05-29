namespace MassTransit.Azure.ServiceBus.Core.Builders
{
    using Configuration;
    using Contexts;
    using MassTransit.Builders;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusSubscriptionEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IServiceBusSubscriptionEndpointConfiguration _configuration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusSubscriptionEndpointBuilder(IServiceBusHostConfiguration hostConfiguration, IServiceBusSubscriptionEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public ServiceBusReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var topologyLayout = BuildTopology(_configuration.Settings);

            var context = new ServiceBusEntityReceiveEndpointContext(_hostConfiguration, _configuration, topologyLayout);

            context.GetOrAddPayload(() => _hostConfiguration.HostTopology);

            return context;
        }

        static BrokerTopology BuildTopology(SubscriptionSettings settings)
        {
            var topologyBuilder = new SubscriptionEndpointBrokerTopologyBuilder();

            topologyBuilder.Topic = topologyBuilder.CreateTopic(settings.TopicDescription);

            topologyBuilder.CreateSubscription(topologyBuilder.Topic, settings.SubscriptionDescription, settings.Rule, settings.Filter);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}
