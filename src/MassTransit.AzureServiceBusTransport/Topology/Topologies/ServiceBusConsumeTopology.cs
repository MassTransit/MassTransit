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
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;


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

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IServiceBusMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}