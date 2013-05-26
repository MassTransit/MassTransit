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
namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pipeline;
    using Saga;
    using StructureMap;


    public class StructureMapSagaRepository<T> :
        ISagaRepository<T>
        where T : class, ISaga
    {
        readonly IContainer _container;
        readonly ISagaRepository<T> _repository;

        public StructureMapSagaRepository(ISagaRepository<T> repository, IContainer container)
        {
            _repository = repository;
            _container = container;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context,
            Guid sagaId, InstanceHandlerSelector<T, TMessage> selector, ISagaPolicy<T, TMessage> policy)
            where TMessage : class
        {
            return _repository.GetSaga(context, sagaId, selector, policy)
                              .Select(consumer => (Action<IConsumeContext<TMessage>>)(x =>
                                  {
                                      using (_container.GetNestedContainer())
                                      {
                                          consumer(x);
                                      }
                                  }));
        }

        public IEnumerable<Guid> Find(ISagaFilter<T> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<T> Where(ISagaFilter<T> filter)
        {
            return _repository.Where(filter);
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<T> filter, Func<T, TResult> transformer)
        {
            return _repository.Where(filter, transformer);
        }

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> transformer)
        {
            return _repository.Select(transformer);
        }
    }
}