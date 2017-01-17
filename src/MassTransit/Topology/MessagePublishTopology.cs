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
namespace MassTransit.Topology
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Util;


    public class MessagePublishTopology<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IMessagePublishTopologyConfigurator
        where TMessage : class
    {
        readonly IList<IMessagePublishTopologyConvention<TMessage>> _conventions;
        readonly IMessageEntityNameFormatter<TMessage> _defaultEntityNameFormatter;
        readonly IList<IMessagePublishTopology<TMessage>> _delegateConfigurations;
        readonly IList<IImplementedMessagePublishTopologyConfigurator<TMessage>> _implementedMessageTypes;
        readonly IList<IMessagePublishTopology<TMessage>> _topologies;

        public MessagePublishTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            if (entityNameFormatter == null)
                throw new ArgumentNullException(nameof(entityNameFormatter));

            EntityNameFormatter = entityNameFormatter;
            _defaultEntityNameFormatter = entityNameFormatter;

            _conventions = new List<IMessagePublishTopologyConvention<TMessage>>();
            _topologies = new List<IMessagePublishTopology<TMessage>>();
            _delegateConfigurations = new List<IMessagePublishTopology<TMessage>>();
            _implementedMessageTypes = new List<IImplementedMessagePublishTopologyConfigurator<TMessage>>();
        }

        IMessagePublishTopologyConfigurator<T> IMessagePublishTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IMessagePublishTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }

        public void Add(IMessagePublishTopology<TMessage> publishTopology)
        {
            _topologies.Add(publishTopology);
        }

        public void AddDelegate(IMessagePublishTopology<TMessage> configuration)
        {
            _delegateConfigurations.Add(configuration);
        }

        public void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
        {
            ITopologyPipeBuilder<PublishContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (IMessagePublishTopology<TMessage> topology in _delegateConfigurations)
            {
                topology.Apply(delegatedBuilder);
            }

            foreach (IMessagePublishTopologyConvention<TMessage> convention in _conventions)
            {
                IMessagePublishTopology<TMessage> topology;
                if (convention.TryGetMessagePublishTopology(out topology))
                    topology.Apply(builder);
            }

            foreach (IMessagePublishTopology<TMessage> topology in _topologies)
            {
                topology.Apply(builder);
            }
        }

        public virtual bool TryGetPublishAddress(Uri baseAddress, TMessage message, out Uri publishAddress)
        {
            publishAddress = null;
            return false;
        }

        public void AddConvention(IMessagePublishTopologyConvention<TMessage> convention)
        {
            _conventions.Add(convention);
        }

        public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessagePublishTopologyConvention<TMessage>
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

        public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter ?? _defaultEntityNameFormatter;
        }

        public void SetEntityName(string entityName)
        {
            EntityNameFormatter = new StaticEntityNameFormatter<TMessage>(entityName);
        }

        public void AddImplementedMessageConfigurator<T>(IMessagePublishTopologyConfigurator<T> configurator)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IImplementedMessagePublishTopologyConfigurator<TMessage>
            where T : class
        {
            readonly IMessagePublishTopologyConfigurator<T> _configurator;

            public ImplementedTypeAdapter(IMessagePublishTopologyConfigurator<T> configurator)
            {
                _configurator = configurator;
            }
        }
    }
}