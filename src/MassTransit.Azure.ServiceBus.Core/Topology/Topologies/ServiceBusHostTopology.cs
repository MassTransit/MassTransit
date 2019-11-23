namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using System.Text;
    using Configuration;
    using Configuration.Configurators;
    using Core.Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Transports;


    public class ServiceBusHostTopology :
        HostTopology,
        IServiceBusHostTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;

        public ServiceBusHostTopology(IServiceBusTopologyConfiguration configuration, Uri hostAddress)
            : base(configuration)
        {
            _configuration = configuration;
            _hostAddress = hostAddress;

            _messageNameFormatter = new ServiceBusMessageNameFormatter();
        }

        IServiceBusPublishTopology IServiceBusHostTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusHostTopology.SendTopology => _configuration.Send;

        public Uri GetDestinationAddress(string queueName, Action<IQueueConfigurator> configure = null)
        {
            var configurator = new QueueConfigurator(queueName);

            configure?.Invoke(configurator);

            return configurator.GetQueueAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IQueueConfigurator> configure = null)
        {
            var queueName = _messageNameFormatter.GetMessageName(messageType).ToString();

            var configurator = new QueueConfigurator(queueName);

            if (TypeMetadataCache.IsTemporaryMessageType(messageType))
            {
                configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
            }

            configure?.Invoke(configurator);

            return configurator.GetQueueAddress(_hostAddress);
        }

        public override string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder();

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            sb.Append('_');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '_')
                    sb.Append(c);
            sb.AppendFormat("_{0}_", prefix);
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }

        IServiceBusMessagePublishTopology<T> IServiceBusHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IServiceBusMessageSendTopology<T> IServiceBusHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }
    }
}
