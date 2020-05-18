namespace MassTransit.Azure.ServiceBus.Core.Builders
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Util;


    public class ServiceBusReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IServiceBusHostControl _host;
        readonly IServiceBusReceiveEndpointConfiguration _configuration;

        public ServiceBusReceiveEndpointBuilder(IServiceBusHostControl host, IServiceBusReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.ConfigureConsumeTopology)
            {
                var subscriptionName = GenerateSubscriptionName();

                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Subscribe(subscriptionName);
            }

            return base.ConnectConsumePipe(pipe);
        }

        public ServiceBusReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var topologyLayout = BuildTopology(_configuration.Settings);

            return new ServiceBusEntityReceiveEndpointContext(_host, _configuration, topologyLayout);
        }

        static readonly char[] Separator = {'/'};

        string GenerateSubscriptionName()
        {
            var subscriptionName = _configuration.Settings.Name.Split(Separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            var suffix = _configuration.HostAddress.AbsolutePath.Split(Separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (!string.IsNullOrWhiteSpace(suffix))
                subscriptionName += $"-{suffix}";

            string name;
            if (subscriptionName.Length > 50)
            {
                string hashed;
                using (var hasher = new SHA1Managed())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(subscriptionName);
                    byte[] hash = hasher.ComputeHash(buffer);
                    hashed = FormatUtil.Formatter.Format(hash).Substring(0, 6);
                }

                name = $"{subscriptionName.Substring(0, 43)}-{hashed}";
            }
            else
                name = subscriptionName;

            return name;
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.CreateQueue(settings.GetQueueDescription());

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}
