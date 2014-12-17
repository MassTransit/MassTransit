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


    public class CorrelatedSagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TSaga, TMessage>> _consumeFilter;
        readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;
        readonly IFilter<ConsumeContext<TMessage>> _locatorFilter;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public CorrelatedSagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter,
            IFilter<ConsumeContext<TMessage>> locatorFilter)
        {
            _consumeFilter = consumeFilter;
            _locatorFilter = locatorFilter;
        }

        protected override IFilter<SagaConsumeContext<TSaga, TMessage>> GetMessageFilter()
        {
            return _consumeFilter;
        }

        protected override IFilter<ConsumeContext<TMessage>> GetLocatorFilter(ISagaRepository<TSaga> repository)
        {
            return _locatorFilter;
        }
    }
}