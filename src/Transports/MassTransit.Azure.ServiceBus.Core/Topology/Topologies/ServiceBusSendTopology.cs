namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using Builders;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Microsoft.Azure.ServiceBus.Management;
    using Settings;
    using Transport;


    public class ServiceBusSendTopology :
        SendTopology,
        IServiceBusSendTopologyConfigurator
    {
        const string ErrorQueueSuffix = "_error";
        const string DeadLetterQueueSuffix = "_skipped";

        public Action<IEntityConfigurator> ConfigureErrorSettings { get; set; }
        public Action<IEntityConfigurator> ConfigureDeadLetterSettings { get; set; }

        IServiceBusMessageSendTopology<T> IServiceBusSendTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessageSendTopologyConfigurator<T>;
        }

        IServiceBusMessageSendTopologyConfigurator<T> IServiceBusSendTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(ServiceBusEndpointAddress address)
        {
            if (address.Type == ServiceBusEndpointAddress.AddressType.Queue)
            {
                var queueDescription = GetQueueDescription(address);

                return new QueueSendSettings(queueDescription);
            }

            var topicDescription = GetTopicDescription(address);

            var builder = new PublishEndpointBrokerTopologyBuilder();
            builder.Topic = builder.CreateTopic(topicDescription);

            return new TopicSendSettings(topicDescription, builder.BuildBrokerTopology());
        }

        public SendSettings GetErrorSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Path = description.Path + ErrorQueueSuffix;

            var errorSettings = new QueueSendSettings(description);

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetErrorSettings(ISubscriptionConfigurator configurator, Uri hostAddress)
        {
            var description = configurator.GetSubscriptionDescription();

            var errorEndpointAddress = new ServiceBusEndpointAddress(hostAddress, description.SubscriptionName + ErrorQueueSuffix);

            var queueDescription = Defaults.CreateQueueDescription(errorEndpointAddress.Path);
            queueDescription.DefaultMessageTimeToLive = description.DefaultMessageTimeToLive;
            queueDescription.AutoDeleteOnIdle = description.AutoDeleteOnIdle;

            var errorSettings = new QueueSendSettings(queueDescription);

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetDeadLetterSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Path = description.Path + DeadLetterQueueSuffix;

            var deadLetterSetting = new QueueSendSettings(description);

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        public SendSettings GetDeadLetterSettings(ISubscriptionConfigurator configurator, Uri hostAddress)
        {
            var description = configurator.GetSubscriptionDescription();

            var deadLetterEndpointAddress = new ServiceBusEndpointAddress(hostAddress, description.SubscriptionName + DeadLetterQueueSuffix);

            var queueDescription = Defaults.CreateQueueDescription(deadLetterEndpointAddress.Path);
            queueDescription.DefaultMessageTimeToLive = description.DefaultMessageTimeToLive;
            queueDescription.AutoDeleteOnIdle = description.AutoDeleteOnIdle;

            var deadLetterSetting = new QueueSendSettings(queueDescription);

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        static QueueDescription GetQueueDescription(ServiceBusEndpointAddress address)
        {
            var queueDescription = Defaults.CreateQueueDescription(address.Path);

            if (address.AutoDelete.HasValue)
                queueDescription.AutoDeleteOnIdle = address.AutoDelete.Value;

            return queueDescription;
        }

        static TopicDescription GetTopicDescription(ServiceBusEndpointAddress address)
        {
            var topicDescription = Defaults.CreateTopicDescription(address.Path);

            if (address.AutoDelete.HasValue)
                topicDescription.AutoDeleteOnIdle = address.AutoDelete.Value;

            return topicDescription;
        }
    }
}
