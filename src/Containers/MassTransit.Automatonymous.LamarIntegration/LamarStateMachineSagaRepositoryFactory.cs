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
namespace MassTransit.AutomatonymousLamarIntegration
{
    using Automatonymous;
    using Saga;
    using Scoping;
    using Lamar;
    using LamarIntegration;


    public class LamarStateMachineSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public LamarStateMachineSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new LamarSagaScopeProvider<T>(_container);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new LamarStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
