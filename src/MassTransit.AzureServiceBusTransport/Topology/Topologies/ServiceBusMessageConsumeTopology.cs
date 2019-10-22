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
namespace MassTransit.AzureServiceBusTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using Configuration.Specifications;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Util;


    public class ServiceBusMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IServiceBusMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IServiceBusMessagePublishTopology<TMessage> publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Subscribe(string subscriptionName, Action<ISubscriptionConfigurator> configure = null)
        {
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subscriptionName));

            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidServiceBusConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var topicDescription = _publishTopology.TopicDescription;

            var subscriptionConfigurator = new SubscriptionConfigurator(topicDescription.Path, subscriptionName);

            configure?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(topicDescription, subscriptionConfigurator.GetSubscriptionDescription(), subscriptionConfigurator.Rule,
                subscriptionConfigurator.Filter);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}