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
namespace MassTransit.LamarIntegration
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Lamar;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class LamarSagaScopeProvider<TSaga> : ISagaScopeProvider<TSaga> where TSaga : class, ISaga
    {
        private readonly IContainer _container;
        private readonly IList<Action<ConsumeContext>> _scopeActions;

        public LamarSagaScopeProvider(IContainer container)
        {
            _container = container;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context) where T : class
        {
            if (context.TryGetPayload<IContainer>(out _))
                return new ExistingSagaScopeContext<T>(context);

            var container = _container.GetNestedContainer(context);
            try
            {
                var proxy = new ConsumeContextProxy<T>(context, new PayloadCacheScope(context));

                var consumerContainer = container;
                proxy.GetOrAddPayload(() => consumerContainer);
                foreach (var scopeAction in _scopeActions)
                {
                    scopeAction(proxy);
                }

                return new CreatedSagaScopeContext<IContainer, T>(consumerContainer, proxy);
            }
            catch
            {
                container.Dispose();
                throw;
            }
        }

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context) where T : class
        {
            if (context.TryGetPayload<IContainer>(out _))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var container = _container.GetNestedContainer(context);
            try
            {
                var proxy = new SagaQueryConsumeContextProxy<TSaga, T>(context, new PayloadCacheScope(context), context.Query);
                var consumerContainer = container;

                proxy.GetOrAddPayload(() => consumerContainer);
                foreach (var scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<IContainer, TSaga, T>(consumerContainer, proxy);
            }
            catch
            {
                container.Dispose();
                throw;
            }
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
