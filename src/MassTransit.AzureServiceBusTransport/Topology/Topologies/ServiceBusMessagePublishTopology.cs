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
        readonly Lazy<TopicDescription> _topicDescription;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            _topicDescription = new Lazy<TopicDescription>(GetTopicDescription);
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

        TopicDescription GetTopicDescription()
        {
            var topicDescription = Defaults.CreateTopicDescription(_messageTopology.EntityName);
            if (TypeMetadataCache<TMessage>.IsTemporaryMessageType)
                topicDescription.EnableExpress = true;

            return topicDescription;
        }
    }
}