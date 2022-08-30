namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Globalization;
    using MassTransit.Topology;


    public class AmazonSqsSendTopology :
        SendTopology,
        IAmazonSqsSendTopologyConfigurator
    {
        public AmazonSqsSendTopology(IEntityNameValidator validator)
        {
            EntityNameValidator = validator;
        }

        public IEntityNameValidator EntityNameValidator { get; }

        public Action<IAmazonSqsQueueConfigurator> ConfigureErrorSettings { get; set; }
        public Action<IAmazonSqsQueueConfigurator> ConfigureDeadLetterSettings { get; set; }

        IAmazonSqsMessageSendTopologyConfigurator<T> IAmazonSqsSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IAmazonSqsMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(AmazonSqsEndpointAddress address)
        {
            return new QueueSendSettings(address);
        }

        public ErrorSettings GetErrorSettings(ReceiveSettings settings)
        {
            var errorSettings = new QueueErrorSettings(settings, BuildEntityName(settings.EntityName, x => ErrorQueueNameFormatter.FormatErrorQueueName(x)));

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public DeadLetterSettings GetDeadLetterSettings(ReceiveSettings settings)
        {
            var deadLetterSetting = new QueueDeadLetterSettings(settings,
                BuildEntityName(settings.EntityName, x => DeadLetterQueueNameFormatter.FormatDeadLetterQueueName(x)));

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        static string BuildEntityName(string entityName, Func<string, string> formatQueueName)
        {
            const string fifoSuffix = ".fifo";

            if (!entityName.EndsWith(fifoSuffix, true, CultureInfo.InvariantCulture))
                return formatQueueName(entityName);

            return formatQueueName(entityName.Substring(0, entityName.Length - fifoSuffix.Length)) + fifoSuffix;
        }
    }
}
