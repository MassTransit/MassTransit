namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using Azure.Messaging.ServiceBus.Administration;


    public class QueueSendSettings :
        SendSettings,
        IServiceBusEntityConfigurator
    {
        readonly CreateQueueOptions _createQueueOptions;

        public QueueSendSettings(CreateQueueOptions createQueueOptions)
        {
            _createQueueOptions = createQueueOptions;
        }

        public TimeSpan? AutoDeleteOnIdle
        {
            set
            {
                if (value.HasValue)
                    _createQueueOptions.AutoDeleteOnIdle = value.Value;
            }
        }

        public TimeSpan? DefaultMessageTimeToLive
        {
            set
            {
                if (value.HasValue)
                    _createQueueOptions.DefaultMessageTimeToLive = value.Value;
            }
        }

        public bool? EnableBatchedOperations
        {
            set
            {
                if (value.HasValue)
                    _createQueueOptions.EnableBatchedOperations = value.Value;
            }
        }

        public string UserMetadata
        {
            set => _createQueueOptions.UserMetadata = value;
        }

        public string EntityPath => _createQueueOptions.Name;

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new SendEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(_createQueueOptions);

            return builder.BuildBrokerTopology();
        }
    }
}
