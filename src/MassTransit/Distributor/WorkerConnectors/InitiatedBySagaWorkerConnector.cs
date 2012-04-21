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
namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using Saga;
    using Saga.Pipeline;

    public class InitiatedBySagaWorkerConnector<TSaga, TMessage> :
        SagaWorkerConnectorBase<TSaga, TMessage>
        where TMessage : class, CorrelatedBy<Guid>
        where TSaga : class, ISaga, InitiatedBy<TMessage>
    {
        public InitiatedBySagaWorkerConnector(ISagaRepository<TSaga> sagaRepository)
            : base(sagaRepository)
        {
        }

        protected override ISagaPolicy<TSaga, TMessage> GetPolicy()
        {
            return new InitiatingSagaPolicy<TSaga, TMessage>(x => x.CorrelationId, x => false);
        }

        protected override ISagaMessageSink<TSaga, TMessage> GetSagaMessageSink(ISagaRepository<TSaga> sagaRepository,
            ISagaPolicy<TSaga, TMessage> policy)
        {
            return new CorrelatedSagaMessageSink<TSaga, TMessage>(sagaRepository, policy);
        }
    }
}