namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


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

        public Action<IRabbitMqQueueBindingConfigurator> ConfigureErrorSettings { get; set; }
        public Action<IRabbitMqQueueBindingConfigurator> ConfigureDeadLetterSettings { get; set; }

        IRabbitMqMessageSendTopologyConfigurator<T> IRabbitMqSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IRabbitMqMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(RabbitMqEndpointAddress address)
        {
            return new RabbitMqSendSettings(address);
        }

        // TODO this is a smell, send for error/dead-letter settings?
        public ErrorSettings GetErrorSettings(ReceiveSettings settings)
        {
            var errorSettings = new RabbitMqErrorSettings(settings, settings.ExchangeName + "_error");

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public DeadLetterSettings GetDeadLetterSettings(ReceiveSettings settings)
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
