namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Settings;


    public class RabbitMqSendTopology :
        SendTopology,
        IRabbitMqSendTopologyConfigurator
    {
        public RabbitMqSendTopology(IEntityNameValidator validator)
        {
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();
            EntityNameValidator = validator;
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }
        public IEntityNameValidator EntityNameValidator { get; }

        public Action<IQueueBindingConfigurator> ConfigureErrorSettings { get; set; }
        public Action<IQueueBindingConfigurator> ConfigureDeadLetterSettings { get; set; }

        IRabbitMqMessageSendTopologyConfigurator<T> IRabbitMqSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IRabbitMqMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(RabbitMqEndpointAddress address)
        {
            return new RabbitMqSendSettings(address);
        }

        public ErrorSettings GetErrorSettings(EntitySettings settings)
        {
            var errorSettings = new RabbitMqErrorSettings(settings, settings.ExchangeName + "_error");

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            var deadLetterSetting = new RabbitMqDeadLetterSettings(settings, settings.ExchangeName + "_skipped");

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new RabbitMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
