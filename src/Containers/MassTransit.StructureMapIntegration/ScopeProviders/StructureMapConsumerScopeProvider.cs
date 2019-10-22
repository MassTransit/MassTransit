namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;
    using StructureMap;
    using Util;


    public class StructureMapConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapConsumerScopeProvider(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public StructureMapConsumerScopeProvider(IContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                return new ExistingConsumerScopeContext(context);
            }

            var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
            try
            {
                var proxy = new ConsumeContextScope(context, nestedContainer);

                return new CreatedConsumerScopeContext<IContainer>(nestedContainer, proxy);
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
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject(context);

                var consumer = existingContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
            try
            {
                var consumer = nestedContainer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerContext = context.PushConsumerScope(consumer, nestedContainer);

                return new CreatedConsumerScopeContext<IContainer, TConsumer, T>(nestedContainer, consumerContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
