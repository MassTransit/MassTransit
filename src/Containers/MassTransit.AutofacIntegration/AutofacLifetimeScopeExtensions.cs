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
namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Util;


    public static class AutofacLifetimeScopeExtensions
    {
        public static ConsumerConsumeContext<TConsumer, TMessage> GetConsumer<TConsumer, TMessage>(this IComponentContext componentContext, ConsumeContext<TMessage> consumeContext)
            where TConsumer : class
            where TMessage : class
        {
            var consumer = componentContext.ResolveOptional<TConsumer>();
            if (consumer == null)
                throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

            return consumeContext.PushConsumer(consumer);
        }

        public static ConsumerConsumeContext<TConsumer, TMessage> GetConsumerScope<TConsumer, TMessage>(this ILifetimeScope lifetimeScope, ConsumeContext<TMessage> consumeContext)
            where TConsumer : class
            where TMessage : class
        {
            var consumer = lifetimeScope.ResolveOptional<TConsumer>();
            if (consumer == null)
                throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

            return consumeContext.PushConsumerScope(consumer, lifetimeScope);
        }

        public static ILifetimeScope GetLifetimeScope<TMessage, TId>(this ILifetimeScopeRegistry<TId> registry, ConsumeContext<TMessage> context)
            where TMessage : class
        {
            var scopeId = GetScopeId(registry, context);

            return registry.GetLifetimeScope(scopeId);
        }

        public static TId GetScopeId<TMessage, TId>(this ILifetimeScopeRegistry<TId> registry, ConsumeContext<TMessage> context)
            where TMessage : class
        {
            var scopeId = default(TId);

            // first, try to use the message-based scopeId provider
            if (registry.TryResolve(out ILifetimeScopeIdAccessor<TMessage, TId> provider) && provider.TryGetScopeId(context.Message, out scopeId))
                return scopeId;

            // second, try to use the consume context based message version
            var idProvider = registry.ResolveOptional<ILifetimeScopeIdProvider<TId>>(TypedParameter.From(context), TypedParameter.From<ConsumeContext>(context));
            if (idProvider != null && idProvider.TryGetScopeId(out scopeId))
                return scopeId;

            // okay, give up, default it is
            return scopeId;
        }

        public static void ConfigureScope(this ContainerBuilder builder, ConsumeContext context)
        {
            builder.RegisterInstance(context)
                .As<ConsumeContext>()
                .As<IPublishEndpoint>()
                .As<ISendEndpointProvider>()
                .ExternallyOwned();
        }

        public static void ConfigureScope<T>(this ContainerBuilder builder, ConsumeContext<T> context)
            where T : class
        {
            builder.RegisterInstance(context)
                .As<ConsumeContext>()
                .As<ConsumeContext<T>>()
                .As<IPublishEndpoint>()
                .As<ISendEndpointProvider>()
                .ExternallyOwned();
        }
    }
}