namespace MassTransit.LamarIntegration
{
    using Context;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Util;


    public class LamarConsumerScopeProvider : IConsumerScopeProvider
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
            if (context.TryGetPayload<IContainer>(out _))
                return new ExistingConsumerScopeContext(context);

            var container = _container.GetNestedContainer(context);
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

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out var existingConsumer))
            {
                var consumer = existingConsumer.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = context.PushConsumer(consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var container = _container.GetNestedContainer(context);
            try
            {
                var consumer = container.GetInstance<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = context.PushConsumerScope(consumer, container);

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
