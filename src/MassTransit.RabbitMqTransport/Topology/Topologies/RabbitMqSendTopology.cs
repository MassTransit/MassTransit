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
            return new RabbitMqErrorSettings(settings, settings.ExchangeName + "_error");
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            return new RabbitMqDeadLetterSettings(settings, settings.ExchangeName + "_skipped");
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new RabbitMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
