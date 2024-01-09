#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public class SqlSendTopology :
        SendTopology,
        ISqlSendTopologyConfigurator
    {
        public Action<ISqlQueueConfigurator>? ConfigureErrorSettings { get; set; }
        public Action<ISqlQueueConfigurator>? ConfigureDeadLetterSettings { get; set; }

        public new ISqlMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return (configurator as ISqlMessageSendTopologyConfigurator<T>)!;
        }

        public SendSettings GetSendSettings(SqlEndpointAddress address)
        {
            return address.Type == SqlEndpointAddress.AddressType.Queue
                ? new QueueSendSettings(address)
                : new TopicSendSettings(address);
        }

        public SendSettings GetErrorSettings(ReceiveSettings settings)
        {
            var errorSettings = new QueueSendSettings(settings, ErrorQueueNameFormatter.FormatErrorQueueName(settings.QueueName));

            ConfigureErrorSettings?.Invoke(errorSettings);

            return errorSettings;
        }

        public SendSettings GetDeadLetterSettings(ReceiveSettings settings)
        {
            var deadLetterSetting = new QueueSendSettings(settings, DeadLetterQueueNameFormatter.FormatDeadLetterQueueName(settings.QueueName));

            ConfigureDeadLetterSettings?.Invoke(deadLetterSetting);

            return deadLetterSetting;
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new SqlMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
