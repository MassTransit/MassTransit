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
namespace MassTransit.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public class MessageSendTopology<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IMessageSendTopologyConvention<TMessage>> _conventions;
        readonly IList<IMessageSendTopology<TMessage>> _delegateConfigurations;
        readonly IList<IMessageSendTopology<TMessage>> _topologies;

        public MessageSendTopology()
        {
            _conventions = new List<IMessageSendTopologyConvention<TMessage>>();
            _topologies = new List<IMessageSendTopology<TMessage>>();
            _delegateConfigurations = new List<IMessageSendTopology<TMessage>>();
        }

        public void Add(IMessageSendTopology<TMessage> sendTopology)
        {
            _topologies.Add(sendTopology);
        }

        public void AddDelegate(IMessageSendTopology<TMessage> configuration)
        {
            _delegateConfigurations.Add(configuration);
        }

        public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
        {
            ITopologyPipeBuilder<SendContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (IMessageSendTopology<TMessage> topology in _delegateConfigurations)
            {
                topology.Apply(delegatedBuilder);
            }

            foreach (IMessageSendTopologyConvention<TMessage> convention in _conventions)
            {
                IMessageSendTopology<TMessage> topology;
                if (convention.TryGetMessageSendTopology(out topology))
                    topology.Apply(builder);
            }

            foreach (IMessageSendTopology<TMessage> topology in _topologies)
            {
                topology.Apply(builder);
            }
        }

        public void AddConvention(IMessageSendTopologyConvention<TMessage> convention)
        {
            _conventions.Add(convention);
        }

        public void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                var convention = _conventions[i] as TConvention;
                if (convention != null)
                {
                    var updatedConvention = update(convention);
                    _conventions[i] = updatedConvention;
                    return;
                }
            }
        }

        public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                var convention = _conventions[i] as TConvention;
                if (convention != null)
                {
                    var updatedConvention = update(convention);
                    _conventions[i] = updatedConvention;
                    return;
                }
            }

            var addedConvention = add();
            if (addedConvention != null)
                _conventions.Add(addedConvention);
        }
    }
}