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
namespace MassTransit.ActiveMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using Entities;
    using GreenPipes;


    /// <summary>
    /// Used to by a Consumer virtual destination to the receive endpoint, via an additional message consumer
    /// </summary>
    public class ConsumerConsumeTopologySpecification :
        TopicBindingConfigurator,
        IActiveMqConsumeTopologySpecification
    {
        readonly string _consumerName;

        public ConsumerConsumeTopologySpecification(string topicName, string consumerName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
            _consumerName = consumerName;
        }

        public ConsumerConsumeTopologySpecification(Topic topic, string consumerName)
            : base(topic)
        {
            _consumerName = consumerName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var consumerQueueName = _consumerName.Replace("{queue}", builder.Queue.Queue.EntityName);

            var queue = builder.CreateQueue(consumerQueueName, Durable, AutoDelete);

            var consumer = builder.BindConsumer(exchangeHandle, queue, Selector);
        }
    }
}