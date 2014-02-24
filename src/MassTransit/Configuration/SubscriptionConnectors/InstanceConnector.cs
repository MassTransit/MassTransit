// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Pipeline;
    using Saga;
    using Util;

    public interface InstanceConnector
    {
        UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance);
    }

    public class InstanceConnector<T> :
        InstanceConnector
        where T : class
    {
        readonly IEnumerable<InstanceSubscriptionConnector> _connectors;

        public InstanceConnector()
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            if (interfaces.Implements(typeof(InitiatedBy<>))
                || interfaces.Implements(typeof(Orchestrates<>))
                || interfaces.Implements(typeof(Observes<,>)))
                throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

            _connectors = ConsumesContext()
                    .Concat(ConsumesAll())
                    .Distinct((x, y) => x.MessageType == y.MessageType)
                    .ToList();
        }


        public IEnumerable<InstanceSubscriptionConnector> Connectors
        {
            get { return _connectors; }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
        {
            return _connectors.Select(x => x.Connect(configurator, instance))
                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
        }

        IEnumerable<InstanceSubscriptionConnector> ConsumesContext()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(CreateContextConnector);
        }

        static InstanceSubscriptionConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(ContextInstanceSubscriptionConnector<,>),
                       new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<InstanceSubscriptionConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<T>.MessageConsumerTypes.Select(CreateConnector);
        }

        static InstanceSubscriptionConnector CreateConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(InstanceSubscriptionConnector<,>), new[] {typeof(T), x.MessageType});
        }
    }
}