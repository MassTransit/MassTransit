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
namespace MassTransit.Saga.SagaConnectors
{
    using System;
    using System.Linq.Expressions;
    using Connectors;
    using Metadata;
    using Pipeline.Filters;
    using Policies;
    using QueryFactories;


    public class ObservesSagaConnectorFactory<TSaga, TMessage> :
        ISagaConnectorFactory
        where TSaga : class, ISaga, Observes<TMessage, TSaga>
        where TMessage : class
    {
        readonly ISagaMessageConnector<TSaga> _connector;

        public ObservesSagaConnectorFactory()
        {
            var policy = new AnyExistingSagaPolicy<TSaga, TMessage>();

            ISagaQueryFactory<TSaga, TMessage> queryFactory = new ExpressionSagaQueryFactory<TSaga, TMessage>(GetFilterExpression());

            var consumeFilter = new ObservesSagaMessageFilter<TSaga, TMessage>();

            _connector = new QuerySagaMessageConnector<TSaga, TMessage>(consumeFilter, policy, queryFactory);
        }

        ISagaMessageConnector<T> ISagaConnectorFactory.CreateMessageConnector<T>()
        {
            var connector = _connector as ISagaMessageConnector<T>;
            if (connector == null)
                throw new ArgumentException("The saga type did not match the connector type");

            return connector;
        }

        static Expression<Func<TSaga, TMessage, bool>> GetFilterExpression()
        {
            var instance = SagaMetadataCache<TSaga>.FactoryMethod(NewId.NextGuid());

            return instance.CorrelationExpression;
        }
    }
}