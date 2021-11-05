namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Util;


    public class SimpleInjectorConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly Container _container;

        public SimpleInjectorConsumerScopeProvider(Container container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public ValueTask<IConsumerScopeContext> GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                return new ValueTask<IConsumerScopeContext>(new ExistingConsumerScopeContext(context));
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var scopeContext = new ConsumeContextScope(context, scope, scope.Container);

                scope.UpdateScope(scopeContext);

                return new ValueTask<IConsumerScopeContext>(new CreatedConsumerScopeContext<Scope>(scope, scopeContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IConsumerScopeContext>(() => scope.DisposeScopeAsync());
            }
        }

        public ValueTask<IConsumerScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                var consumer = existingScope.Container.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(new ExistingConsumerScopeContext<TConsumer, T>(consumerContext));
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var scopeContext = new ConsumeContextScope<T>(context, scope, scope.Container);

                scope.UpdateScope(scopeContext);

                var consumer = scope.Container.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(scopeContext, consumer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(new CreatedConsumerScopeContext<Scope, TConsumer, T>(scope, consumerContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IConsumerScopeContext<TConsumer, T>>(() => scope.DisposeScopeAsync());
            }
        }
    }
}
