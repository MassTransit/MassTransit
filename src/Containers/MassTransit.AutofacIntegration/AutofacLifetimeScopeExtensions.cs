namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Context;
    using GreenPipes;
    using Metadata;


    public static class AutofacLifetimeScopeExtensions
    {
        public static ConsumerConsumeContext<TConsumer, TMessage> GetConsumer<TConsumer, TMessage>(this IComponentContext componentContext,
            ConsumeContext<TMessage> consumeContext)
            where TConsumer : class
            where TMessage : class
        {
            var consumer = componentContext.ResolveOptional<TConsumer>();
            if (consumer == null)
                throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

            return new ConsumerConsumeContextScope<TConsumer, TMessage>(consumeContext, consumer);
        }

        public static ConsumerConsumeContext<TConsumer, TMessage> GetConsumerScope<TConsumer, TMessage>(this ILifetimeScope lifetimeScope,
            ConsumeContext<TMessage> consumeContext)
            where TConsumer : class
            where TMessage : class
        {
            var consumer = lifetimeScope.ResolveOptional<TConsumer>();
            if (consumer == null)
                throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

            return new ConsumerConsumeContextScope<TConsumer, TMessage>(consumeContext, consumer, lifetimeScope);
        }

        public static SendContext<TMessage> GetSendScope<TMessage>(this ILifetimeScope lifetimeScope,
            SendContext<TMessage> sendContext)
            where TMessage : class
        {
            return new SendContextScope<TMessage>(sendContext, lifetimeScope);
        }

        public static PublishContext<TMessage> GetPublishScope<TMessage>(this ILifetimeScope lifetimeScope,
            PublishContext<TMessage> publishContext)
            where TMessage : class
        {
            return new PublishContextScope<TMessage>(publishContext, lifetimeScope);
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
            var idProvider =
                registry.ResolveOptional<ILifetimeScopeIdProvider<TId>>(TypedParameter.From(context), TypedParameter.From<ConsumeContext>(context));

            if (idProvider != null && idProvider.TryGetScopeId(out scopeId))
                return scopeId;

            // okay, give up, default it is
            return scopeId;
        }

        public static void ConfigureScope<T>(this ContainerBuilder builder, SendContext<T> context)
            where T : class
        {
            builder.RegisterInstance(context)
                .As<SendContext>()
                .ExternallyOwned();
        }

        public static void ConfigureScope<T>(this ContainerBuilder builder, PublishContext<T> context)
            where T : class
        {
            builder.RegisterInstance(context)
                .As<PublishContext>()
                .ExternallyOwned();
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

        public static void UpdatePayload(this PipeContext context, ILifetimeScope scope)
        {
            context.GetOrAddPayload(() => scope);
        }
    }
}
