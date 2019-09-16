// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Microsoft.Azure.ServiceBus.Management;
    using Settings;
    using Transport;
    using Util;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IServiceBusMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topicConfigurator;
        readonly Lazy<TopicDescription> _topicDescription;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            _topicDescription = new Lazy<TopicDescription>(GetTopicDescription);

            _topicConfigurator = new TopicConfigurator(messageTopology.EntityName, TypeMetadataCache<TMessage>.IsTemporaryMessageType);
            _implementedMessageTypes = new List<IServiceBusMessagePublishTopology>();
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            var entityName = _messageTopology.EntityName;

            var builder = new UriBuilder(baseAddress)
            {
                Path = entityName
            };

            publishAddress = builder.Uri;
            return true;
        }

        public TopicDescription TopicDescription => _topicDescription.Value;

        public SendSettings GetSendSettings()
        {
            var description = GetTopicDescription();

            var builder = new PublishEndpointBrokerTopologyBuilder();

            Apply(builder);

            var sendSettings = new TopicSendSettings(description, builder.BuildBrokerTopology());

            return sendSettings;
        }

        string IMessageEntityConfigurator.Path => _topicConfigurator.Path;

        string IMessageEntityConfigurator.BasePath
        {
            get => _topicConfigurator.BasePath;
            set => _topicConfigurator.BasePath = value;
        }

        string IMessageEntityConfigurator.FullPath => _topicConfigurator.FullPath;

        TimeSpan? IMessageEntityConfigurator.DuplicateDetectionHistoryTimeWindow
        {
            set => _topicConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        bool? IMessageEntityConfigurator.EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        long? IMessageEntityConfigurator.MaxSizeInMB
        {
            set => _topicConfigurator.MaxSizeInMB = value;
        }

        bool? IMessageEntityConfigurator.RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        void IMessageEntityConfigurator.EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _topicConfigurator.EnableDuplicateDetection(historyTimeWindow);
        }

        TimeSpan? IEntityConfigurator.AutoDeleteOnIdle
        {
            set => _topicConfigurator.AutoDeleteOnIdle = value;
        }

        TimeSpan? IEntityConfigurator.DefaultMessageTimeToLive
        {
            set => _topicConfigurator.DefaultMessageTimeToLive = value;
        }

        bool? IEntityConfigurator.EnableBatchedOperations
        {
            set => _topicConfigurator.EnableBatchedOperations = value;
        }

        string IEntityConfigurator.UserMetadata
        {
            set => _topicConfigurator.UserMetadata = value;
        }

        bool? ITopicConfigurator.EnableFilteringMessagesBeforePublishing
        {
            set => _topicConfigurator.EnableFilteringMessagesBeforePublishing = value;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(_topicDescription.Value);

            builder.Topic = topicHandle;

            foreach (IServiceBusMessagePublishTopology configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        TopicDescription GetTopicDescription()
        {
            return _topicConfigurator.GetTopicDescription();
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
