// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    /// <summary>
    ///     Interface implemented by objects that tie an inbound pipeline together with
    ///     consumers (by means of calling a consumer factory).
    /// </summary>
    public interface ConsumerConnector
    {
        UnsubscribeAction Connect<TConsumer>(IInboundPipelineConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class;
    }


    public class ConsumerConnector<T> :
        ConsumerConnector
        where T : class
    {
        readonly IEnumerable<ConsumerSubscriptionConnector> _connectors;

        public ConsumerConnector()
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

        public IEnumerable<ConsumerSubscriptionConnector> Connectors
        {
            get { return _connectors; }
        }

        public UnsubscribeAction Connect<TConsumer>(IInboundPipelineConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class
        {
            return _connectors.Select(x => x.Connect(configurator, consumerFactory))
                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
        }

        static IEnumerable<ConsumerSubscriptionConnector> ConsumesContext()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(CreateContextConnector);
        }

        static ConsumerSubscriptionConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (ConsumerSubscriptionConnector)
                FastActivator.Create(typeof(ContextConsumerSubscriptionConnector<,>),
                    new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<ConsumerSubscriptionConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<T>.MessageConsumerTypes.Select(CreateConnector);
        }

        static ConsumerSubscriptionConnector CreateConnector(MessageInterfaceType x)
        {
            return (ConsumerSubscriptionConnector)
                FastActivator.Create(typeof(ConsumerSubscriptionConnector<,>),
                    new[] {typeof(T), x.MessageType});
        }
    }
}