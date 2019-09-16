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
namespace MassTransit.Transports.InMemory.Topology.Topologies
{
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using GreenPipes;
    using InMemory.Builders;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Specifications;
    using Util;


    public class InMemoryMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryMessageConsumeTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind()
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidInMemoryConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ExchangeBindingConsumeTopologySpecification(_messageTopology.EntityName);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}