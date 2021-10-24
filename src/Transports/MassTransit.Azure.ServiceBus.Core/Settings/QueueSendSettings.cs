namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using global::Azure.Messaging.ServiceBus.Administration;
    using Topology;
    using Topology.Builders;
    using Transport;


    public class QueueSendSettings :
        SendSettings,
        IEntityConfigurator
    {
        readonly CreateQueueOptions _description;

        public QueueSendSettings(CreateQueueOptions description)
        {
            _description = description;
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

        public string EntityPath => _description.Name;

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new SendEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(_description);

            return builder.BuildBrokerTopology();
        }
    }
}
