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
namespace MassTransit.StructureMapIntegration
{
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.ConsumerContexts;
    using StructureMap;
    using Util;


    public class StructureMapConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IContainer _container;

        public StructureMapConsumerScopeProvider(IContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
                return new ExistingConsumerScopeContext(context);

            var container = _container.CreateNestedContainer(context);
            try
            {
                var proxy = new ConsumeContextProxyScope(context);

                var consumerContainer = container;
                proxy.GetOrAddPayload(() => consumerContainer);

                return new CreatedConsumerScopeContext<IContainer>(consumerContainer, proxy);
            }
            catch
            {
                container.Dispose();

                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context) where TConsumer : class where T : class
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                var consumer = existingContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var container = _container.CreateNestedContainer(context);
            try
            {
                var consumer = container.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumerScope(consumer, container);

                return new CreatedConsumerScopeContext<IContainer, TConsumer, T>(container, consumerContext);
            }
            catch
            {
                container.Dispose();

                throw;
            }
        }
    }
}