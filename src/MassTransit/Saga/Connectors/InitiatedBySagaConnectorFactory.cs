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
namespace MassTransit.Saga.Connectors
{
    using System;
    using Pipeline.Filters;
    using Policies;


    public class InitiatedBySagaConnectorFactory<TSaga, TMessage> :
        SagaConnectorFactory
        where TSaga : class, ISaga, InitiatedBy<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly InitiatedBySagaMessageFilter<TSaga, TMessage> _consumeFilter;
        readonly SagaLocatorFilter<TSaga, TMessage> _locatorFilter;

        public InitiatedBySagaConnectorFactory()
        {
            ISagaPolicy<TSaga, TMessage> policy = new InitiatingSagaPolicy<TSaga, TMessage>(x => x.Message.CorrelationId);

            _consumeFilter = new InitiatedBySagaMessageFilter<TSaga, TMessage>();

            var locator = new CorrelationIdSagaLocator<TMessage>(x => x.Message.CorrelationId);

            _locatorFilter = new SagaLocatorFilter<TSaga, TMessage>(locator, policy);
        }

        public SagaMessageConnector CreateMessageConnector()
        {
            return new CorrelatedSagaMessageConnector<TSaga, TMessage>(_consumeFilter, _locatorFilter);
        }
    }
}