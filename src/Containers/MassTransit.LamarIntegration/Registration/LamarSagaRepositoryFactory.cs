// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Lamar;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class LamarSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;
        readonly Action<ConsumeContext> _configureScope;

        public LamarSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        public LamarSagaRepositoryFactory(IContainer container, Action<ConsumeContext> configureScope)
        {
            _container = container;
            _configureScope = configureScope;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new LamarSagaScopeProvider<T>(_container);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            if (_configureScope != null)
                scopeProvider.AddScopeAction(_configureScope);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
