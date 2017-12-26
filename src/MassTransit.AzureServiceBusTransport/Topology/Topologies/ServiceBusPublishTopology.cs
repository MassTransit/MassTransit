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
    using Configuration;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;


    public class ServiceBusPublishTopology :
        PublishTopology,
        IServiceBusPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public ServiceBusPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IServiceBusMessagePublishTopology<T> IServiceBusPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        IServiceBusMessagePublishTopologyConfigurator<T> IServiceBusPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}