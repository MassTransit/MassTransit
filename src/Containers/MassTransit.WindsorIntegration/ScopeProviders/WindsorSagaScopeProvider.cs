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
namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Castle.MicroKernel;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;


    public class WindsorSagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IKernel _kernel;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public WindsorSagaScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public ISagaScopeContext<T> GetScope<T>(ConsumeContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingSagaScopeContext<T>(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope();
            try
            {
                _kernel.UpdateScope(context);

                var proxy = new ConsumeContextProxy<T>(context, new PayloadCacheScope(context));
                proxy.UpdatePayload(_kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaScopeContext<IDisposable, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }

        public ISagaQueryScopeContext<TSaga, T> GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingSagaQueryScopeContext<TSaga, T>(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope();
            try
            {
                _kernel.UpdateScope(context);

                var proxy = new SagaQueryConsumeContextProxy<TSaga, T>(context, new PayloadCacheScope(context), context.Query);
                proxy.UpdatePayload(_kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedSagaQueryScopeContext<IDisposable, TSaga, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }
    }
}
