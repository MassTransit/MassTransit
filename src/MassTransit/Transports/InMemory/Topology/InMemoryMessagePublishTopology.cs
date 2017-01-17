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
namespace MassTransit.Transports.InMemory.Topology
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Topology;
    using Util;


    public class InMemoryMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IInMemoryPublishTopologySpecification> _implementedMessageTypes;

        public InMemoryMessagePublishTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
            : base(entityNameFormatter)
        {
            _implementedMessageTypes = new List<IInMemoryPublishTopologySpecification>();
        }

        IInMemoryMessagePublishTopologyConfigurator<T> IInMemoryMessagePublishTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IInMemoryMessagePublishTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public void Apply(IInMemoryPublishTopologyBuilder builder)
        {
            var exchangeHandle = ExchangeDeclare(builder);

            if (builder.ExchangeName != null)
            {
                builder.ExchangeBind(builder.ExchangeName, exchangeHandle);
            }
            else
            {
                builder.ExchangeName = exchangeHandle;
            }

            foreach (IInMemoryPublishTopologySpecification configurator in _implementedMessageTypes)
            {
                configurator.Apply(builder);
            }
        }

        public override bool TryGetPublishAddress(Uri baseAddress, TMessage message, out Uri publishAddress)
        {
            var exchangeName = EntityNameFormatter.FormatEntityName();

            publishAddress = new Uri($"{baseAddress}{exchangeName}");
            return true;
        }

        string ExchangeDeclare(IInMemoryTopologyBuilder builder)
        {
            var exchangeName = EntityNameFormatter.FormatEntityName();

            builder.ExchangeDeclare(exchangeName);

            return exchangeName;
        }

        public void AddImplementedMessageConfigurator<T>(IInMemoryMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new TypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class TypeAdapter<T> :
            IInMemoryPublishTopologySpecification
            where T : class
        {
            readonly IInMemoryMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public TypeAdapter(IInMemoryMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IInMemoryPublishTopologyBuilder builder)
            {
                if (_direct)
                {
                    var implementedBuilder = builder.CreateImplementedBuilder();

                    _configurator.Apply(implementedBuilder);
                }
            }
        }
    }
}