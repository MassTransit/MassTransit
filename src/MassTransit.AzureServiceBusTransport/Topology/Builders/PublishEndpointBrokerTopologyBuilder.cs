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
namespace MassTransit.AzureServiceBusTransport.Topology.Builders
{
    using System;
    using Entities;
    using Microsoft.ServiceBus.Messaging;


    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        [Flags]
        public enum Options
        {
            FlattenHierarchy = 0,
            MaintainHierarchy = 1
        }


        readonly Options _options;

        public PublishEndpointBrokerTopologyBuilder(Options options = Options.FlattenHierarchy)
        {
            _options = options;
        }

        /// <summary>
        /// The topic where the published message is sent
        /// </summary>
        public TopicHandle Topic { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(Options.MaintainHierarchy))
            {
                return new ImplementedBuilder(this, _options);
            }

            return this;
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new ServiceBusBrokerTopology(Topics, Subscriptions, Queues, QueueSubscriptions, TopicSubscriptions);
        }


        class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            readonly Options _options;
            TopicHandle _topic;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder, Options options)
            {
                _builder = builder;
                _options = options;
            }

            public TopicHandle Topic
            {
                get { return _topic; }
                set
                {
                    _topic = value;
                    if (_builder.Topic != null)
                    {
                        var subscriptionDescription = new SubscriptionDescription(_builder.Topic.Topic.TopicDescription.Path, NewId.NextGuid().ToString("N"))
                        {
                            ForwardTo = value.Topic.TopicDescription.Path
                        };

                        _builder.CreateTopicSubscription(_builder.Topic, _topic, subscriptionDescription);
                    }
                }
            }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                if (_options.HasFlag(Options.MaintainHierarchy))
                {
                    return new ImplementedBuilder(this, _options);
                }

                return this;
            }

            public TopicHandle CreateTopic(TopicDescription topicDescription)
            {
                return _builder.CreateTopic(topicDescription);
            }

            public SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription)
            {
                return _builder.CreateSubscription(topic, subscriptionDescription);
            }

            public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription)
            {
                return _builder.CreateTopicSubscription(source, destination, subscriptionDescription);
            }

            public QueueHandle CreateQueue(QueueDescription queueDescription)
            {
                return _builder.CreateQueue(queueDescription);
            }

            public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription)
            {
                return _builder.CreateQueueSubscription(exchange, queue, subscriptionDescription);
            }
        }
    }
}