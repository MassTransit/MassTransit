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
namespace MassTransit.UnityIntegration
{
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Unity;
    using Util;


    public class UnityConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IUnityContainer _container;

        public UnityConsumerScopeProvider(IUnityContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "unity");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingScope))
                return new ExistingConsumerScopeContext(context);

            var scope = _container.CreateChildContainer();
            try
            {
                var proxy = new ConsumeContextProxyScope(context);

                var consumerScope = scope;
                proxy.GetOrAddPayload(() => consumerScope);

                return new CreatedConsumerScopeContext<IUnityContainer>(consumerScope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context) where TConsumer : class where T : class
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingScope))
            {
                var consumer = existingScope.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var scope = _container.CreateChildContainer();
            try
            {
                var consumer = scope.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumerScope(consumer, scope);

                return new CreatedConsumerScopeContext<IUnityContainer, TConsumer, T>(scope, consumerContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}