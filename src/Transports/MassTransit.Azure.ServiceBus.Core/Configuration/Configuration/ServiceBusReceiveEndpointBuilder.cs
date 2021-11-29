namespace MassTransit.Configuration
{
    using System;
    using System.Linq;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using AzureServiceBusTransport.Topology;


    public class ServiceBusReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        static readonly char[] Separator = { '/' };
        readonly IServiceBusReceiveEndpointConfiguration _configuration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusReceiveEndpointBuilder(IServiceBusHostConfiguration hostConfiguration, IServiceBusReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                IServiceBusMessageConsumeTopologyConfigurator<T> topology = _configuration.Topology.Consume.GetMessageTopology<T>();
                if (topology.ConfigureConsumeTopology)
                {
                    var subscriptionName = GenerateSubscriptionName();
                    topology.Subscribe(subscriptionName);
                }
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public ServiceBusReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var topologyLayout = BuildTopology(_configuration.Settings);

            return new ServiceBusEntityReceiveEndpointContext(_hostConfiguration, _configuration, topologyLayout, ClientContextFactory);
        }

        string GenerateSubscriptionName()
        {
            var subscriptionName = _configuration.Settings.Name.Split(Separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var hostScope = _configuration.HostAddress.AbsolutePath.Split(Separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            return _configuration.Topology.Publish.GenerateSubscriptionName(subscriptionName, hostScope);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.CreateQueue(settings.GetCreateQueueOptions());

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }

        IClientContextSupervisor ClientContextFactory()
        {
            return _hostConfiguration.ConnectionContextSupervisor
                .CreateClientContextSupervisor(supervisor => new QueueClientContextFactory(supervisor, _configuration.Settings));
        }
    }
}
