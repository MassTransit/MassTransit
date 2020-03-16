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
namespace MassTransit.AmazonSqsTransport.Topology.Configuration.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Entities;
    using Configurators;
    using GreenPipes;


    /// <summary>
    /// Used to by a TopicSubscription destination to the receive endpoint, via an additional message consumer
    /// </summary>
    public class ConsumerConsumeTopologySpecification :
        TopicSubscriptionConfigurator,
        IAmazonSqsConsumeTopologySpecification
    {
        public ConsumerConsumeTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public ConsumerConsumeTopologySpecification(Topic topic)
            : base(topic)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(EntityName, Durable, AutoDelete, TopicAttributes, TopicSubscriptionAttributes, Tags);

            var topicSubscriptionHandle = builder.CreateQueueSubscription(topicHandle, builder.Queue);
        }
    }
}
