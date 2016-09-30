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
namespace MassTransit.Saga.Connectors
{
    using GreenPipes;
    using MassTransit.Pipeline;
    using Pipeline.Filters;


    public class QuerySagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TSaga, TMessage>> _consumeFilter;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly ISagaQueryFactory<TSaga, TMessage> _queryFactory;

        public QuerySagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter, ISagaPolicy<TSaga, TMessage> policy,
            ISagaQueryFactory<TSaga, TMessage> queryFactory)
        {
            _consumeFilter = consumeFilter;
            _policy = policy;
            _queryFactory = queryFactory;
        }

        protected override void ConfigureSagaPipe(IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> configurator)
        {
            configurator.UseFilter(_consumeFilter);
        }

        protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
        {
            configurator.UseFilter(new QuerySagaFilter<TSaga, TMessage>(repository, _policy, _queryFactory, sagaPipe));
        }
    }
}