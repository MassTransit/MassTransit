namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


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

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                return new ExistingConsumerScopeContext(context);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context);

                var proxy = new ConsumeContextScope(context, scope, scope.Container);

                return new CreatedConsumerScopeContext<Scope>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
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

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context);

                var consumer = scope.Container.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer, scope, scope.Container);

                return new CreatedConsumerScopeContext<Scope, TConsumer, T>(scope, consumerContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
