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
                var createQueueOptions = GetCreateQueueOptions(address);

                return new QueueSendSettings(createQueueOptions);
            }

            var createTopicOptions = GetCreateTopicOptions(address);

            var builder = new BrokerTopologyBuilder();
            builder.CreateTopic(createTopicOptions);

            return new TopicSendSettings(createTopicOptions, builder.BuildBrokerTopology());
        }

        public SendSettings GetErrorSettings(IQueueConfigurator configurator)
        {
            var createQueueOptions = configurator.GetCreateQueueOptions();
            createQueueOptions.Name += ErrorQueueSuffix;

            var errorSettings = new QueueSendSettings(createQueueOptions);

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetDeadLetterSettings(IQueueConfigurator configurator)
        {
            var createQueueOptions = configurator.GetCreateQueueOptions();
            createQueueOptions.Name += DeadLetterQueueSuffix;

            var deadLetterSetting = new QueueSendSettings(createQueueOptions);

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        static CreateQueueOptions GetCreateQueueOptions(ServiceBusEndpointAddress address)
        {
            var createQueueOptions = Defaults.GetCreateQueueOptions(address.Path);

            if (address.AutoDelete.HasValue)
                createQueueOptions.AutoDeleteOnIdle = address.AutoDelete.Value;

            return createQueueOptions;
        }

        static CreateTopicOptions GetCreateTopicOptions(ServiceBusEndpointAddress address)
        {
            var createTopicOptions = Defaults.GetCreateTopicOptions(address.Path);

            if (address.AutoDelete.HasValue)
                createTopicOptions.AutoDeleteOnIdle = address.AutoDelete.Value;

            return createTopicOptions;
        }
    }
}
