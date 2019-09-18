namespace MassTransit.Azure.ServiceBus.Core.Builders
{
    using Configuration;
    using Contexts;
    using MassTransit.Builders;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusSubscriptionEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusHostControl _host;
        readonly IServiceBusSubscriptionEndpointConfiguration _configuration;

        public ServiceBusSubscriptionEndpointBuilder(IServiceBusHostControl host, IServiceBusSubscriptionEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
        }

        public ServiceBusReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var topologyLayout = BuildTopology(_configuration.Settings);

            return new ServiceBusEntityReceiveEndpointContext(_host, _configuration, topologyLayout);
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
