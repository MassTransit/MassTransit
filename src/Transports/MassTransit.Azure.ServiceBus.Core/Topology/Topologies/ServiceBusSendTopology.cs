namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using Builders;
    using global::Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
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

            var builder = new BrokerTopologyBuilder();
            builder.CreateTopic(topicDescription);

            return new TopicSendSettings(topicDescription, builder.BuildBrokerTopology());
        }

        public SendSettings GetErrorSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Name += ErrorQueueSuffix;

            var errorSettings = new QueueSendSettings(description);

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetDeadLetterSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Name += DeadLetterQueueSuffix;

            var deadLetterSetting = new QueueSendSettings(description);

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        static CreateQueueOptions GetQueueDescription(ServiceBusEndpointAddress address)
        {
            var queueDescription = Defaults.CreateQueueDescription(address.Path);

            if (address.AutoDelete.HasValue)
                queueDescription.AutoDeleteOnIdle = address.AutoDelete.Value;

            return queueDescription;
        }

        static CreateTopicOptions GetTopicDescription(ServiceBusEndpointAddress address)
        {
            var topicDescription = Defaults.CreateTopicDescription(address.Path);

            if (address.AutoDelete.HasValue)
                topicDescription.AutoDeleteOnIdle = address.AutoDelete.Value;

            return topicDescription;
        }
    }
}
