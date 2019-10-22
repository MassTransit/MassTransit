namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System.Text;
    using Core.Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class ServiceBusHostTopology :
        HostTopology,
        IServiceBusHostTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;

        public ServiceBusHostTopology(IServiceBusTopologyConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        IServiceBusPublishTopology IServiceBusHostTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusHostTopology.SendTopology => _configuration.Send;

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
