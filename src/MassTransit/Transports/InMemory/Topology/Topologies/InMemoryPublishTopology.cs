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
namespace MassTransit.Transports.InMemory.Topology.Topologies
{
    using System;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class InMemoryPublishTopology :
        PublishTopology,
        IInMemoryPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public InMemoryPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IInMemoryMessagePublishTopology<T> IInMemoryPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessagePublishTopology<T>;
        }

        IInMemoryMessagePublishTopologyConfigurator<T> IInMemoryPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new InMemoryMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, topology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(topology);

            return topology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly InMemoryMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IInMemoryPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IInMemoryPublishTopologyConfigurator publishTopology,
                InMemoryMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IInMemoryMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}