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
namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using Configuration.Specifications;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Util;


    public class ServiceBusConsumeTopology :
        ConsumeTopology,
        IServiceBusConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusConsumeTopology(IMessageTopology messageTopology, IServiceBusPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        IServiceBusMessageConsumeTopology<T> IServiceBusConsumeTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessageConsumeTopologyConfigurator<T>;
        }

        IServiceBusMessageConsumeTopologyConfigurator<T> IServiceBusConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IServiceBusConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        public void Subscribe(string topicName, string subscriptionName, Action<ISubscriptionConfigurator> callback = null)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicName));

            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subscriptionName));

            var topicDescription = Defaults.CreateTopicDescription(topicName);

            var subscriptionConfigurator = new SubscriptionConfigurator(topicDescription.Path, subscriptionName);

            callback?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(topicDescription, subscriptionConfigurator.GetSubscriptionDescription(),
                subscriptionConfigurator.Rule,
                subscriptionConfigurator.Filter);

            _specifications.Add(specification);
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IServiceBusMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
