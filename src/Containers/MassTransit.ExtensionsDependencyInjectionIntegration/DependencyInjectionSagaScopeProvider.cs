// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class DependencyInjectionSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IList<Action<ConsumeContext>> _scopeActions;
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        ISagaScopeContext<T> ISagaScopeProvider<TSaga>.GetScope<T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
                return new ExistingSagaScopeContext<T>(context);

            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            try
            {
                var proxy = new ConsumeContextProxy<T>(context, new PayloadCacheScope(context));

                var sagaScope = scope;
                proxy.GetOrAddPayload(() => sagaScope);
                proxy.GetOrAddPayload(() => sagaScope.ServiceProvider);
                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<IServiceScope, T>(sagaScope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        ISagaQueryScopeContext<TSaga, T> ISagaScopeProvider<TSaga>.GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            try
            {
                var proxy = new SagaQueryConsumeContextProxy<TSaga, T>(context, new PayloadCacheScope(context), context.Query);

                var sagaScope = scope;
                proxy.GetOrAddPayload(() => sagaScope);
                proxy.GetOrAddPayload(() => sagaScope.ServiceProvider);
                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<IServiceScope, TSaga, T>(sagaScope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}