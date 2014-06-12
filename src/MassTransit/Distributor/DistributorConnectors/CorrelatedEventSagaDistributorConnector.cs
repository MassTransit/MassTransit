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
    using System.Linq.Expressions;
    using Magnum.StateMachine;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using Pipeline;
    using Saga;
    using Saga.Configuration;

    public class CorrelatedEventSagaDistributorConnector<TSaga, TMessage> :
        SagaDistributorConnector
        where TSaga : SagaStateMachine<TSaga>, ISaga
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly DataEvent<TSaga, TMessage> _dataEvent;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly IWorkerSelectorFactory _workerSelectorFactory;

        public CorrelatedEventSagaDistributorConnector(IWorkerSelectorFactory workerSelectorFactory,
            ISagaRepository<TSaga> sagaRepository,
            DataEvent<TSaga, TMessage> dataEvent,
            IEnumerable<State> states,
            ISagaPolicyFactory policyFactory,
            Expression<Func<TSaga, bool>> removeExpression)
        {
            _workerSelectorFactory = workerSelectorFactory;
            _sagaRepository = sagaRepository;
            _dataEvent = dataEvent;

            Func<TMessage, Guid> getNewSagaId = message => message.CorrelationId;

            _policy = policyFactory.GetPolicy(states, getNewSagaId, removeExpression);
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            IWorkerAvailability<TMessage> workerAvailability = distributor.GetWorkerAvailability<TMessage>();

            // TODO we need to make a saga worker availability so that we can split saga load by correlation id
            IWorkerSelector<TMessage> workerSelector = _workerSelectorFactory.GetSelector<TMessage>();

            var sink = new DistributorMessageSink<TMessage>(workerAvailability, workerSelector);
            throw new NotImplementedException();

//            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
        }
    }
}