namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Lamar;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Util;


    public class LamarConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IContainer _container;

        public LamarConsumerScopeProvider(IContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                return new ExistingConsumerScopeContext(context);
            }

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var proxy = new ConsumeContextScope(context, nestedContainer);

                return new CreatedConsumerScopeContext<INestedContainer>(nestedContainer, proxy);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                var consumer = existingContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var consumer = nestedContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumerScope(consumer, nestedContainer);

                return new CreatedConsumerScopeContext<INestedContainer, TConsumer, T>(nestedContainer, consumerContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
