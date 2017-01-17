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
namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;


    public class ServiceBusConsumeTopology :
        ConsumeTopology,
        IServiceBusConsumeTopologyConfigurator
    {
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusConsumeTopology(IEntityNameFormatter entityNameFormatter)
            : base(entityNameFormatter)
        {
            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        IServiceBusMessageConsumeTopologyConfigurator<T> IServiceBusConsumeTopology.GetMessageTopology<T>()
        {
            IMessageConsumeTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IServiceBusMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
            {
                specification.Apply(builder);
            }

            ForEach<IServiceBusMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Subscribe(string topicPath, string subscriptionName, Action<ISubscriptionConfigurator> configure = null)
        {
            var topicConfigurator = new TopicConfigurator(topicPath);

            var subscriptionConfigurator = new SubscriptionConfigurator(topicPath, subscriptionName);

            configure?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(topicConfigurator.GetTopicDescription(),
                subscriptionConfigurator.GetSubscriptionDescription());

            _specifications.Add(specification);
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var entityNameFormatter = new MessageEntityNameFormatter<T>(EntityNameFormatter);

            var messageTopology = new ServiceBusMessageConsumeTopology<T>(entityNameFormatter);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}