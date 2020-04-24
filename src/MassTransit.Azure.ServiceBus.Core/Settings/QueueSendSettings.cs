namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using Microsoft.Azure.ServiceBus.Management;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class QueueSendSettings :
        SendSettings,
        IEntityConfigurator
    {
        readonly QueueDescription _description;

        public QueueSendSettings(QueueDescription description)
        {
            _description = description;
        }

        public string EntityPath => _description.Path;

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new SendEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(_description);

            return builder.BuildBrokerTopology();
        }

        public TimeSpan? AutoDeleteOnIdle
        {
            set
            {
                if (value.HasValue)
                    _description.AutoDeleteOnIdle = value.Value;
            }
        }

        public TimeSpan? DefaultMessageTimeToLive
        {
            set
            {
                if (value.HasValue)
                    _description.DefaultMessageTimeToLive = value.Value;
            }
        }

        public bool? EnableBatchedOperations
        {
            set
            {
                if (value.HasValue)
                    _description.EnableBatchedOperations = value.Value;
            }
        }

        public string UserMetadata
        {
            set => _description.UserMetadata = value;
        }
    }
}
