namespace MassTransit.Azure.ServiceBus.Core.Builders
{
    using Configuration;
    using Contexts;
    using MassTransit.Builders;
    using Pipeline;
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

            return new ServiceBusEntityReceiveEndpointContext(_hostConfiguration, _configuration, topologyLayout, ClientContextFactory);
        }

        static BrokerTopology BuildTopology(SubscriptionSettings settings)
        {
            var topologyBuilder = new SubscriptionEndpointBrokerTopologyBuilder();

            topologyBuilder.Topic = topologyBuilder.CreateTopic(settings.CreateTopicOptions);

            topologyBuilder.CreateSubscription(topologyBuilder.Topic, settings.CreateSubscriptionOptions, settings.Rule, settings.Filter);

            return topologyBuilder.BuildBrokerTopology();
        }

        IClientContextSupervisor ClientContextFactory()
        {
            return _hostConfiguration.ConnectionContextSupervisor
                .CreateClientContextSupervisor(supervisor => new SubscriptionClientContextFactory(supervisor, _configuration.Settings));
        }
    }
}
