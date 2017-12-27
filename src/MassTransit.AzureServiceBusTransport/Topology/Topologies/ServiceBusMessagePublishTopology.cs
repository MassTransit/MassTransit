// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Topologies
{
    using System;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topicConfigurator;
        readonly Lazy<TopicDescription> _topicDescription;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            _topicDescription = new Lazy<TopicDescription>(GetTopicDescription);

            _topicConfigurator = new TopicConfigurator(messageTopology.EntityName, TypeMetadataCache<TMessage>.IsTemporaryMessageType);
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

        string IMessageEntityConfigurator.Path => _topicConfigurator.Path;

        TimeSpan? IMessageEntityConfigurator.DuplicateDetectionHistoryTimeWindow
        {
            set => _topicConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        bool? IMessageEntityConfigurator.EnableExpress
        {
            set => _topicConfigurator.EnableExpress = value;
        }

        bool? IMessageEntityConfigurator.EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        bool? IMessageEntityConfigurator.IsAnonymousAccessible
        {
            set => _topicConfigurator.IsAnonymousAccessible = value;
        }

        long? IMessageEntityConfigurator.MaxSizeInMegabytes
        {
            set => _topicConfigurator.MaxSizeInMegabytes = value;
        }

        bool? IMessageEntityConfigurator.RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        bool? IMessageEntityConfigurator.SupportOrdering
        {
            set => _topicConfigurator.SupportOrdering = value;
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

        TopicDescription GetTopicDescription()
        {
            return _topicConfigurator.GetTopicDescription();
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(_topicDescription.Value);

            if (builder.Topic == null)
                builder.Topic = topicHandle;

/*
            builder.CreateTopicSubscription(builder.Topic, topicHandle, new SubscriptionConfigurator(builder.Topic.Topic.TopicDescription.Path, topicHandle));
            
                builder.ExchangeBind(builder.Exchange, exchangeHandle, "", new Dictionary<string, object>());
            else
                builder.Exchange = exchangeHandle;

            foreach (IRabbitMqMessagePublishTopology configurator in _implementedMessageTypes)
                configurator.Apply(builder);
*/
        }
    }
}