namespace MassTransit.AzureServiceBusTransport.Builders
{
    using System;
    using MassTransit.Builders;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class ServiceBusSubscriptionEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IServiceBusHost _host;
        readonly IServiceBusEndpointConfiguration _configuration;
        BusHostCollection<ServiceBusHost> _hosts;

        public ServiceBusSubscriptionEndpointBuilder(IBusBuilder busBuilder, IServiceBusHost host, BusHostCollection<ServiceBusHost> hosts, IServiceBusEndpointConfiguration configuration)
            : base(busBuilder, configuration)
        {
            _configuration = configuration;
            _host = host;
            _hosts = hosts;
        }

        public IServiceBusReceiveEndpointTopology CreateReceiveEndpointTopology(Uri inputAddress, SubscriptionSettings settings)
        {
            var topologyLayout = BuildTopology(settings);

            return new ServiceBusReceiveEndpointTopology(_configuration, inputAddress, MessageSerializer, _host, _hosts, topologyLayout);
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