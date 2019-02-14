// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AmazonSqsTransport.Configuration;
    using Builders;
    using Configuration;
    using Configuration.Specifications;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Util;


    public class AmazonSqsConsumeTopology :
        ConsumeTopology,
        IAmazonSqsConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IAmazonSqsPublishTopology _publishTopology;
        readonly IList<IAmazonSqsConsumeTopologySpecification> _specifications;

        public AmazonSqsConsumeTopology(IMessageTopology messageTopology, IAmazonSqsPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IAmazonSqsConsumeTopologySpecification>();
        }

        IAmazonSqsMessageConsumeTopology<T> IAmazonSqsConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IAmazonSqsMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IAmazonSqsConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IAmazonSqsMessageConsumeTopologyConfigurator<T> IAmazonSqsConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IAmazonSqsMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IAmazonSqsMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<ITopicSubscriptionConfigurator> configure = null)
        {
            var specification = new ConsumerConsumeTopologySpecification(topicName);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
