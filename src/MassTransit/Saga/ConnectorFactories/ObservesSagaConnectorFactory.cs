// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.ConnectorFactories
{
    using System;
    using System.Linq.Expressions;
    using Connectors;
    using Pipeline.Filters;
    using Policies;


    public class ObservesSagaConnectorFactory<TSaga, TMessage> :
        ISagaConnectorFactory
        where TSaga : class, ISaga, Observes<TMessage, TSaga>
        where TMessage : class
    {
        readonly ObservesSagaMessageFilter<TSaga, TMessage> _consumeFilter;
        readonly AnyExistingSagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaQueryFactory<TSaga, TMessage> _queryFactory;

        public ObservesSagaConnectorFactory()
        {
            _policy = new AnyExistingSagaPolicy<TSaga, TMessage>();

            _queryFactory = new ExpressionSagaQueryFactory<TSaga, TMessage>(GetFilterExpression());

            _consumeFilter = new ObservesSagaMessageFilter<TSaga, TMessage>();
        }

        public ISagaMessageConnector CreateMessageConnector()
        {
            return new QuerySagaMessageConnector<TSaga, TMessage>(_consumeFilter, _policy, _queryFactory);
        }

        static Expression<Func<TSaga, TMessage, bool>> GetFilterExpression()
        {
            TSaga instance = SagaMetadataCache<TSaga>.FactoryMethod(NewId.NextGuid());

            return instance.CorrelationExpression;
        }
    }
}