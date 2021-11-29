namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using MassTransit.Topology;
    using Metadata;
    using Util;


    public class ServiceBusPublishTopology :
        PublishTopology,
        IServiceBusPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public ServiceBusPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IServiceBusMessagePublishTopology<T> IServiceBusPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        public string FormatSubscriptionName(string subscriptionName)
        {
            string name;
            if (subscriptionName.Length > 50)
            {
                string hashed;
                using (var hasher = new SHA1Managed())
                {
                    var buffer = Encoding.UTF8.GetBytes(subscriptionName);
                    var hash = hasher.ComputeHash(buffer);
                    hashed = FormatUtil.Formatter.Format(hash).Substring(0, 6);
                }

                name = $"{subscriptionName.Substring(0, 43)}-{hashed}";
            }
            else
                name = subscriptionName;

            return name;
        }

        public string GenerateSubscriptionName(string entityName, string hostScope)
        {
            if (entityName == null)
                throw new ArgumentNullException(nameof(entityName));

            return FormatSubscriptionName(string.IsNullOrWhiteSpace(hostScope) ? entityName : $"{entityName}-{hostScope}");
        }

        IServiceBusMessagePublishTopologyConfigurator<T> IServiceBusPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>(), this);

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly ServiceBusMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IServiceBusPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IServiceBusPublishTopologyConfigurator publishTopology,
                ServiceBusMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IServiceBusMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
