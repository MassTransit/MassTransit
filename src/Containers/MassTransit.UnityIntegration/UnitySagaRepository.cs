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
namespace MassTransit.UnityIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Pipeline;
    using Saga;


    public class UnitySagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IUnityContainer _container;
        readonly ISagaRepository<TSaga> _repository;

        public UnitySagaRepository(ISagaRepository<TSaga> repository, IUnityContainer container)
        {
            _repository = repository;
            _container = container;
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (_container.CreateChildContainer())
            {
                await _repository.Send(context, next);
            }
        }

        IEnumerable<Guid> ISagaRepository<TSaga>.Find(ISagaFilter<TSaga> filter)
        {
            return _repository.Find(filter);
        }
    }
}