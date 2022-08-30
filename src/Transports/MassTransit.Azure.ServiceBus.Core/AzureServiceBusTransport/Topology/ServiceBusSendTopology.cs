namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Topology;


    public class ServiceBusSendTopology :
        SendTopology,
        IServiceBusSendTopologyConfigurator
    {
        public Action<IServiceBusEntityConfigurator> ConfigureErrorSettings { get; set; }
        public Action<IServiceBusEntityConfigurator> ConfigureDeadLetterSettings { get; set; }

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

        public SendSettings GetErrorSettings(IServiceBusQueueConfigurator configurator)
        {
            var createQueueOptions = configurator.GetCreateQueueOptions();
            createQueueOptions.Name = ErrorQueueNameFormatter.FormatErrorQueueName(createQueueOptions.Name);

            var errorSettings = new QueueSendSettings(createQueueOptions);

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetDeadLetterSettings(IServiceBusQueueConfigurator configurator)
        {
            var createQueueOptions = configurator.GetCreateQueueOptions();
            createQueueOptions.Name = DeadLetterQueueNameFormatter.FormatDeadLetterQueueName(createQueueOptions.Name);

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
