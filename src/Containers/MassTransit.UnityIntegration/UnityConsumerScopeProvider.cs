namespace MassTransit.UnityIntegration
{
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Unity;


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
                var proxy = new ConsumeContextScope(context, scope);

                return new CreatedConsumerScopeContext<IUnityContainer>(scope, proxy);
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
            if (context.TryGetPayload<IUnityContainer>(out var existingScope))
            {
                var consumer = existingScope.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var scope = _container.CreateChildContainer();
            try
            {
                var consumer = scope.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer, scope);

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
