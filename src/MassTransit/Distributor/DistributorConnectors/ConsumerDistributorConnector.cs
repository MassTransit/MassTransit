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
namespace MassTransit.Distributor.DistributorConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using MassTransit.SubscriptionConnectors;
    using Saga;
    using Subscriptions;
    using Util;

    public class ConsumerDistributorConnector<T> :
        DistributorConnector
        where T : class
    {
        readonly IEnumerable<MessageDistributorConnector> _connectors;
        readonly ReferenceFactory _referenceFactory;
        readonly IWorkerSelectorFactory _workerSelectorFactory;

        public ConsumerDistributorConnector(ReferenceFactory referenceFactory,
            IWorkerSelectorFactory workerSelectorFactory)
        {
            _workerSelectorFactory = workerSelectorFactory;
            _referenceFactory = referenceFactory;

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

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            throw new NotImplementedException();
//            return _referenceFactory(_connectors.Select(x => x.Connect(configurator, distributor))
//                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x()));
        }

        IEnumerable<MessageDistributorConnector> ConsumesContext()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(CreateConnector);
        }

        IEnumerable<MessageDistributorConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<T>.MessageConsumerTypes.Select(CreateConnector);
        }

        MessageDistributorConnector CreateConnector(MessageInterfaceType x)
        {
            return (MessageDistributorConnector)
                   FastActivator.Create(typeof(MessageDistributorConnector<>),
                       new[] {x.MessageType}, new object[] {_workerSelectorFactory});
        }

        public ISubscriptionReference Connect(IInboundPipe filter, IDistributor distributor)
        {
            var handle = new MultipleConnectHandle(_connectors.Select(x => x.Connect(filter, distributor)));
            return _referenceFactory(handle);
        }
    }
}