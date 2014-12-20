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
namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pipeline;
    using Saga;
    using StructureMap;


    public class StructureMapSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainer _container;
        readonly ISagaRepository<TSaga> _repository;

        public StructureMapSagaRepository(ISagaRepository<TSaga> repository, IContainer container)
        {
            _repository = repository;
            _container = container;
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (_container.GetNestedContainer())
            {
                await _repository.Send(context, next);
            }
        }

        public IEnumerable<Guid> Find(ISagaFilter<TSaga> filter)
        {
            return _repository.Find(filter);
        }
    }
}