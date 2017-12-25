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
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;


    public class InMemoryConsumeTopology :
        ConsumeTopology,
        IInMemoryConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryConsumeTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        IInMemoryMessageConsumeTopology<T> IInMemoryConsumeTopology.GetMessageTopology<T>()
        {
            IMessageConsumeTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IInMemoryMessageConsumeTopology<T>;
        }

        IInMemoryMessageConsumeTopologyConfigurator<T> IInMemoryConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IInMemoryMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new InMemoryMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(topology);

            return topology;
        }
    }
}