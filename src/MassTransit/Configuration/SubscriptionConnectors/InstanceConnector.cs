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

            _connectors = /* Distributors()
                .Concat(Workers())
                .Concat(*/
                ConsumesCorrelated()
                .Concat(ConsumesSelectedContext())
                .Concat(ConsumesContext())
#if !NET35
                .Concat(ConsumesAsyncContext())
                .Concat(ConsumesAsync())
#endif
                .Concat(ConsumesSelected())
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
            return MessageInterfaceTypeReflector<T>.GetConsumesContextTypes()
                .Select(CreateContextConnector);
        }

        IEnumerable<InstanceSubscriptionConnector> ConsumesSelectedContext()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesSelectedContextTypes()
                .Select(CreateSelectedContextConnector);
        }

        static InstanceSubscriptionConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(ContextInstanceSubscriptionConnector<,>),
                       new[] {typeof(T), x.MessageType});
        }

#if !NET35
        static InstanceSubscriptionConnector CreateAsyncContextConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(AsyncContextInstanceSubscriptionConnector<,>),
                       new[] { typeof(T), x.MessageType });
        }

        IEnumerable<InstanceSubscriptionConnector> ConsumesAsyncContext()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesAsyncContextTypes()
                .Select(CreateAsyncContextConnector);
        }
#endif

        static InstanceSubscriptionConnector CreateSelectedContextConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(SelectedContextInstanceSubscriptionConnector<,>),
                       new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<InstanceSubscriptionConnector> ConsumesAll()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesAllTypes()
                .Select(CreateConnector);
        }

        static IEnumerable<InstanceSubscriptionConnector> ConsumesAsync()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesAsyncTypes()
                .Select(CreateAsyncConnector);
        }

        static InstanceSubscriptionConnector CreateConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(InstanceSubscriptionConnector<,>), new[] {typeof(T), x.MessageType});
        }

        static InstanceSubscriptionConnector CreateAsyncConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(AsyncInstanceSubscriptionConnector<,>), new[] { typeof(T), x.MessageType });
        }

        static IEnumerable<InstanceSubscriptionConnector> ConsumesSelected()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesSelectedTypes()
                .Select(CreateSelectedConnector);
        }

        static InstanceSubscriptionConnector CreateSelectedConnector(MessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(SelectedInstanceSubscriptionConnector<,>),
                       new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<InstanceSubscriptionConnector> ConsumesCorrelated()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesCorrelatedTypes()
                .Select(CreateCorrelatedConnector);
        }

        static InstanceSubscriptionConnector CreateCorrelatedConnector(CorrelatedMessageInterfaceType x)
        {
            return (InstanceSubscriptionConnector)
                   FastActivator.Create(typeof(CorrelatedInstanceSubscriptionConnector<,,>),
                       new[] {typeof(T), x.MessageType, x.CorrelationType});
        }


//        static IEnumerable<InstanceSubscriptionConnector> Distributors()
//        {
//            return MessageInterfaceTypeReflector<T>.GetDistributorTypes()
//                .Select(x => FastActivator.Create(typeof (DistributorSubscriptionConnector<>), new[] {x.MessageType}))
//                .Cast<InstanceSubscriptionConnector>();
//        }
//
//        static IEnumerable<InstanceSubscriptionConnector> Workers()
//        {
//            return MessageInterfaceTypeReflector<T>.GetWorkerTypes()
//                .Select(x => FastActivator.Create(typeof (WorkerSubscriptionConnector<>), new[] {x.MessageType}))
//                .Cast<InstanceSubscriptionConnector>();
//        }
    }
}