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
namespace MassTransit.Saga.SubscriptionConnectors
{
    using System;
    using System.Linq.Expressions;
    using MassTransit.Pipeline;
    using Pipeline.Filters;


    public class PropertySagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TSaga, TMessage>> _consumeFilter;
        readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public PropertySagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter, ISagaPolicy<TSaga, TMessage> policy,
            Expression<Func<TSaga, TMessage, bool>> filterExpression)
        {
            _consumeFilter = consumeFilter;
            _policy = policy;
            _filterExpression = filterExpression;
        }

        protected override IFilter<SagaConsumeContext<TSaga, TMessage>> GetMessageFilter()
        {
            return _consumeFilter;
        }

        protected override IFilter<ConsumeContext<TMessage>> GetLocatorFilter(ISagaRepository<TSaga> repository)
        {
            var locator = new PropertySagaLocator<TSaga, TMessage>(repository, _policy, _filterExpression);
            return new SagaLocatorFilter<TSaga, TMessage>(locator, _policy);
        }
    }
}