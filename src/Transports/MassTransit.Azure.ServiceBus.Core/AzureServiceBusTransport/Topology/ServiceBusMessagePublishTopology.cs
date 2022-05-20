#nullable enable
namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Configuration;
    using MassTransit.Topology;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly Lazy<CreateTopicOptions> _createTopicOptions;
        readonly IList<IServiceBusMessagePublishTopology> _implementedMessageTypes;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly ServiceBusTopicConfigurator _topicConfigurator;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology, IServiceBusPublishTopology publishTopology)
        {
            _publishTopology = publishTopology;

            _topicConfigurator = new ServiceBusTopicConfigurator(messageTopology.EntityName, MessageTypeCache<TMessage>.IsTemporaryMessageType);
            _implementedMessageTypes = new List<IServiceBusMessagePublishTopology>();

            _createTopicOptions = new Lazy<CreateTopicOptions>(() => _topicConfigurator.GetCreateTopicOptions());
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
        {
            publishAddress = new ServiceBusEndpointAddress(new Uri(baseAddress.GetLeftPart(UriPartial.Authority)), _topicConfigurator.FullPath);
            return true;
        }

        public CreateTopicOptions CreateTopicOptions => _createTopicOptions.Value;

        public SendSettings GetSendSettings()
        {
            var createTopicOptions = _topicConfigurator.GetCreateTopicOptions();

            var builder = new PublishEndpointBrokerTopologyBuilder(_publishTopology);

            Apply(builder);

            return new TopicSendSettings(createTopicOptions, builder.BuildBrokerTopology());
        }

        public ServiceBusSubscriptionConfigurator GetSubscriptionConfigurator(string subscriptionName)
        {
            return new ServiceBusSubscriptionConfigurator(_publishTopology.FormatSubscriptionName(subscriptionName), CreateTopicOptions.Name);
        }

        public string Path => _topicConfigurator.Path;

        public string BasePath
        {
            get => _topicConfigurator.BasePath;
            set => _topicConfigurator.BasePath = value;
        }

        public string FullPath => _topicConfigurator.FullPath;

        public TimeSpan? DuplicateDetectionHistoryTimeWindow
        {
            set => _topicConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        public bool? EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        public long? MaxSizeInMegabytes
        {
            set => _topicConfigurator.MaxSizeInMegabytes = value;
        }

        public long? MaxMessageSizeInKilobytes
        {
            set => _topicConfigurator.MaxMessageSizeInKilobytes = value;
        }

        public bool? RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _topicConfigurator.EnableDuplicateDetection(historyTimeWindow);
        }

        public TimeSpan? AutoDeleteOnIdle
        {
            set => _topicConfigurator.AutoDeleteOnIdle = value;
        }

        public TimeSpan? DefaultMessageTimeToLive
        {
            set => _topicConfigurator.DefaultMessageTimeToLive = value;
        }

        public bool? EnableBatchedOperations
        {
            set => _topicConfigurator.EnableBatchedOperations = value;
        }

        public string UserMetadata
        {
            set => _topicConfigurator.UserMetadata = value;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            if (Exclude)
                return;

            var topicHandle = builder.CreateTopic(_createTopicOptions.Value);

            builder.Topic = topicHandle;

            foreach (var configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        public void AddImplementedMessageConfigurator<T>(IServiceBusMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IServiceBusMessagePublishTopology
            where T : class
        {
            readonly IServiceBusMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(IServiceBusMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
            {
                if (_direct)
                {
                    var implementedBuilder = builder.CreateImplementedBuilder();

                    _configurator.Apply(implementedBuilder);
                }
            }
        }
    }
}
