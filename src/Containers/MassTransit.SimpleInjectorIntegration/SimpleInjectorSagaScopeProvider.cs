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
namespace MassTransit.SimpleInjectorIntegration
{
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Container _container;

        public SimpleInjectorSagaScopeProvider(Container container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context) where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
                return new ExistingSagaScopeContext<T>(context);

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var proxy = new ConsumeContextProxy<T>(context, new PayloadCacheScope(context));

                var consumerScope = scope;
                proxy.GetOrAddPayload(() => consumerScope);

                return new CreatedSagaScopeContext<Scope, T>(consumerScope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context) where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var proxy = new SagaQueryConsumeContextProxy<TSaga, T>(context, new PayloadCacheScope(context), context.Query);

                var sagaScope = scope;
                proxy.GetOrAddPayload(() => sagaScope);

                return new CreatedSagaQueryScopeContext<Scope, TSaga, T>(sagaScope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}