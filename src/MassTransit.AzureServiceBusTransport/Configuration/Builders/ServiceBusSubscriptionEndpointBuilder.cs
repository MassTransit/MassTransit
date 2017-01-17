namespace MassTransit.AzureServiceBusTransport.Builders
{
    using System;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class ServiceBusSubscriptionEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusHost _host;
        readonly IServiceBusEndpointConfiguration _configuration;

        public ServiceBusSubscriptionEndpointBuilder(IBusBuilder busBuilder, IServiceBusHost host,
            IServiceBusEndpointConfiguration configuration)
            : base(busBuilder, configuration)
        {
            _configuration = configuration;
            _host = host;
        }

        public IServiceBusReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress, SubscriptionSettings settings)
        {
            var topologyLayout = BuildTopology(settings);

            return new ServiceBusReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, SendTransportProvider, _host, topologyLayout);
        }

        TopologyLayout BuildTopology(SubscriptionSettings settings)
        {
            var topologyBuilder = new SubscriptionEndpointConsumeTopologyBuilder();

            topologyBuilder.Topic = topologyBuilder.CreateTopic(settings.TopicDescription);

            topologyBuilder.CreateSubscription(topologyBuilder.Topic, settings.SubscriptionDescription);

            return topologyBuilder.BuildTopologyLayout();
        }
    }
}