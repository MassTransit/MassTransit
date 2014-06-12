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
    using Magnum.Reflection;
    using Pipeline;
    using Pipeline.Sinks;
    using Saga;
    using Util;


    public interface InstanceConnector
    {
        ConnectHandle Connect(IInboundMessagePipe pipe, object instance, IMessageRetryPolicy retryPolicy);
    }


    public class InstanceConnector<T> :
        InstanceConnector
        where T : class
    {
        readonly IEnumerable<InstanceMessageConnector> _connectors;

        public InstanceConnector()
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            if (interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                || interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                || interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(Observes<,>)))
                throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

            _connectors = Consumes()
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToArray();

//            _connectors = ConsumesContext()
//                .Concat(ConsumesAll())
//                .Distinct((x, y) => x.MessageType == y.MessageType)
//                .ToList();
        }

        public ConnectHandle Connect(IInboundMessagePipe pipe, object instance, IMessageRetryPolicy retryPolicy)
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.Connect(pipe, instance, retryPolicy)));
        }

        IEnumerable<InstanceMessageConnector> Consumes()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(x => x.GetInstanceMessageConnector());
        }

        IEnumerable<InstanceMessageConnector> ConsumesContext()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(CreateContextConnector);
        }

        static InstanceMessageConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (InstanceMessageConnector)
                FastActivator.Create(typeof(ContextInstanceSubscriptionConnector<,>),
                    new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<InstanceMessageConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<T>.MessageConsumerTypes.Select(CreateConnector);
        }

        static InstanceMessageConnector CreateConnector(MessageInterfaceType x)
        {
            return (InstanceMessageConnector)
                FastActivator.Create(typeof(InstanceMessageConnector<,>), new[] {typeof(T), x.MessageType});
        }
    }
}