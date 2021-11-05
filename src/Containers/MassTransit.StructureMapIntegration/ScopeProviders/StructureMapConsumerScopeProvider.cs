namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using StructureMap;


    public class StructureMapConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IContainer _container;

        public StructureMapConsumerScopeProvider(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
        }

        public ValueTask<IConsumerScopeContext> GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                return new ValueTask<IConsumerScopeContext>(new ExistingConsumerScopeContext(context));
            }

            var nestedContainer = _container.CreateNestedContainer(context);
            try
            {
                var proxy = new ConsumeContextScope(context, nestedContainer);

                return new ValueTask<IConsumerScopeContext>(new CreatedConsumerScopeContext<IContainer>(nestedContainer, proxy));
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }

        public ValueTask<IConsumerScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                var consumer = existingContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(new ExistingConsumerScopeContext<TConsumer, T>(consumerContext));
            }

            var nestedContainer = _container.CreateNestedContainer(context);
            try
            {
                var consumer = nestedContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer, nestedContainer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(
                    new CreatedConsumerScopeContext<IContainer, TConsumer, T>(nestedContainer, consumerContext));
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
