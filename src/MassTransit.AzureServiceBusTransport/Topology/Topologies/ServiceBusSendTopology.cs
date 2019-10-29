namespace MassTransit.AzureServiceBusTransport.Topology.Topologies
{
    using System;
    using Configuration;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Microsoft.ServiceBus.Messaging;
    using Settings;
    using Transport;


    public class ServiceBusSendTopology :
        SendTopology,
        IServiceBusSendTopologyConfigurator
    {
        const string ErrorQueueSuffix = "_error";
        const string DeadLetterQueueSuffix = "_skipped";

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
            var queueDescription = GetQueueDescription(address);

            return new QueueSendSettings(queueDescription);
        }

        public SendSettings GetErrorSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Path = description.Path + ErrorQueueSuffix;

            return new QueueSendSettings(description);
        }

        public SendSettings GetErrorSettings(ISubscriptionConfigurator configurator, Uri hostAddress)
        {
            var description = configurator.GetSubscriptionDescription();

            var errorEndpointAddress = new ServiceBusEndpointAddress(hostAddress, description.Name + ErrorQueueSuffix);

            var queueDescription = Defaults.CreateQueueDescription(errorEndpointAddress.Path);
            queueDescription.DefaultMessageTimeToLive = description.DefaultMessageTimeToLive;
            queueDescription.AutoDeleteOnIdle = description.AutoDeleteOnIdle;

            return new QueueSendSettings(queueDescription);
        }

        public SendSettings GetDeadLetterSettings(IQueueConfigurator configurator)
        {
            var description = configurator.GetQueueDescription();
            description.Path = description.Path + DeadLetterQueueSuffix;

            return new QueueSendSettings(description);
        }

        public SendSettings GetDeadLetterSettings(ISubscriptionConfigurator configurator, Uri hostAddress)
        {
            var description = configurator.GetSubscriptionDescription();

            var deadLetterEndpointAddress = new ServiceBusEndpointAddress(hostAddress, description.Name + DeadLetterQueueSuffix);

            var queueDescription = Defaults.CreateQueueDescription(deadLetterEndpointAddress.Path);
            queueDescription.DefaultMessageTimeToLive = description.DefaultMessageTimeToLive;
            queueDescription.AutoDeleteOnIdle = description.AutoDeleteOnIdle;

            return new QueueSendSettings(queueDescription);
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
            queueDescription.EnableExpress = address.Express;

            return queueDescription;
        }
    }
}
