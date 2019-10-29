namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Settings;


    public class ActiveMqSendTopology :
        SendTopology,
        IActiveMqSendTopologyConfigurator
    {
        public ActiveMqSendTopology(IEntityNameValidator validator)
        {
            EntityNameValidator = validator;
        }

        public IEntityNameValidator EntityNameValidator { get; }

        IActiveMqMessageSendTopologyConfigurator<T> IActiveMqSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IActiveMqMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(ActiveMqEndpointAddress address)
        {
            if (address.Type == ActiveMqEndpointAddress.AddressType.Queue)
                return new QueueSendSettings(address);

            return new TopicSendSettings(address);
        }

        public ErrorSettings GetErrorSettings(EntitySettings settings)
        {
            return new ActiveMqErrorSettings(settings, settings.EntityName + "_error");
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            return new ActiveMqDeadLetterSettings(settings, settings.EntityName + "_skipped");
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
