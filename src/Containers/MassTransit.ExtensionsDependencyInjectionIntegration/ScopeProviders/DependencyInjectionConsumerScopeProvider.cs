namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.ConsumerContexts;


    public class DependencyInjectionConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionConsumerScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        IConsumerScopeContext IConsumerScopeProvider.GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                return new ExistingConsumerScopeContext(context);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                serviceScope.UpdateScope(context);

                var consumeContext = new ConsumeContextScope(context, serviceScope, serviceScope.ServiceProvider);

                return new CreatedConsumerScopeContext<IServiceScope>(serviceScope, consumeContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }

        IConsumerScopeContext<TConsumer, T> IConsumerScopeProvider.GetScope<TConsumer, T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                var consumer = existingServiceScope.ServiceProvider.GetService<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                serviceScope.UpdateScope(context);

                var consumer = serviceScope.ServiceProvider.GetService<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer, serviceScope, serviceScope.ServiceProvider);

                return new CreatedConsumerScopeContext<IServiceScope, TConsumer, T>(serviceScope, consumerContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }
    }
}
