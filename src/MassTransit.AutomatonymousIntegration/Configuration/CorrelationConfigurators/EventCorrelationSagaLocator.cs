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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Saga;


    /// <summary>
    /// Uses an expression to find the matching saga instances
    /// </summary>
    /// <typeparam name="TInstance">The saga type</typeparam>
    /// <typeparam name="TData">The message type</typeparam>
    public class EventCorrelationSagaLocator<TInstance, TData> :
        ISagaLocator<TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, ConsumeContext<TData>, bool>> _correlationExpression;
        readonly ISagaFilterFactory<TInstance, TData> _filterFactory;
        readonly ISagaPolicy<TInstance, TData> _policy;
        readonly ISagaRepository<TInstance> _repository;

        public EventCorrelationSagaLocator(ISagaRepository<TInstance> repository, ISagaFilterFactory<TInstance, TData> filterFactory,
            ISagaPolicy<TInstance, TData> policy)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
            _filterFactory = filterFactory;
            _policy = policy;
        }

        async Task<IEnumerable<Guid>> ISagaLocator<TData>.Find(ConsumeContext<TData> context)
        {
            ISagaFilter<TInstance> sagaFilter = _filterFactory.GetFilter(context);

            Guid[] sagaIds = (await _repository.Find(sagaFilter)).ToArray();
            if (sagaIds.Length > 0)
                return sagaIds;

            if (_policy.CanCreateInstance(context))
                return Enumerable.Repeat(_policy.GetNewSagaId(context), 1);

            return Enumerable.Empty<Guid>();
        }
    }
}