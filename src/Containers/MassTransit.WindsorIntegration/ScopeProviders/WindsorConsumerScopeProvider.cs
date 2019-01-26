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
    using Scoping;
    using Scoping.ConsumerContexts;
    using Util;


    public class WindsorConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IKernel _kernel;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public WindsorConsumerScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingConsumerScopeContext(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope();
            try
            {
                _kernel.UpdateScope(context);

                var proxy = new ConsumeContextProxyScope(context);
                proxy.UpdatePayload(_kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedConsumerScopeContext<IDisposable>(scope, proxy);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                var consumer = kernel.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext, ReleaseComponent);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope();
            try
            {
                _kernel.UpdateScope(context);

                var consumer = _kernel.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = context.PushConsumerScope(consumer, scope);
                consumerContext.UpdatePayload(_kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(consumerContext);

                return new CreatedConsumerScopeContext<IDisposable, TConsumer, T>(scope, consumerContext, ReleaseComponent);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }

        void ReleaseComponent<T>(T component)
        {
            _kernel.ReleaseComponent(component);
        }
    }
}
