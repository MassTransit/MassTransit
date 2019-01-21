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
namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Entities;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Util;


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

        public PublishEndpointBrokerTopologyBuilder(Options options = Options.MaintainHierarchy)
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
                get => _topic;
                set
                {
                    _topic = value;
                    if (_builder.Topic != null)
                    {
                        var subscriptionName = string.Join("-", value.Topic.TopicDescription.Path.Split('/').Reverse());
                        var subscriptionDescription = new SubscriptionDescription(_builder.Topic.Topic.TopicDescription.Path,
                            GenerateSubscriptionName(subscriptionName))
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

            public SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription, RuleDescription rule,
                Filter filter)
            {
                return _builder.CreateSubscription(topic, subscriptionDescription, rule, filter);
            }

            public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription)
            {
                return _builder.CreateTopicSubscription(source, destination, subscriptionDescription);
            }

            public QueueHandle CreateQueue(QueueDescription queueDescription)
            {
                return _builder.CreateQueue(queueDescription);
            }

            public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription,
                RuleDescription rule,
                Filter filter)
            {
                return _builder.CreateQueueSubscription(exchange, queue, subscriptionDescription, rule, filter);
            }

            string GenerateSubscriptionName(string subscriptionName)
            {
                string name;
                if (subscriptionName.Length > 50)
                {
                    string hashed;
                    using (var hasher = new SHA1Managed())
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(subscriptionName);
                        byte[] hash = hasher.ComputeHash(buffer);
                        hashed = FormatUtil.Formatter.Format(hash).Substring(0, 6);
                    }

                    name = $"{subscriptionName.Substring(0, 43)}-{hashed}";
                }
                else
                    name = subscriptionName;

                return name;
            }
        }
    }
}
