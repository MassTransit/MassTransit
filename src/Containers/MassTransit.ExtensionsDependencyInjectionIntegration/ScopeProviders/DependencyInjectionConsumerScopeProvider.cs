namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Util;


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

        public ValueTask<IConsumerScopeContext> GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.SetCurrentConsumeContext(context);

                return new ValueTask<IConsumerScopeContext>(new ExistingConsumerScopeContext(context));
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                var scopeContext = new ConsumeContextScope(context, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);

                serviceScope.SetCurrentConsumeContext(scopeContext);

                return new ValueTask<IConsumerScopeContext>(new CreatedConsumerScopeContext<IServiceScope>(serviceScope, scopeContext));
            }
            catch (Exception ex)
            {
                if (serviceScope is IAsyncDisposable asyncDisposable)
                    return ex.DisposeAsync<IConsumerScopeContext>(() => asyncDisposable.DisposeAsync());

                serviceScope.Dispose();
                throw;
            }
        }

        public ValueTask<IConsumerScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.SetCurrentConsumeContext(context);

                var consumer = existingServiceScope.ServiceProvider.GetService<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(new ExistingConsumerScopeContext<TConsumer, T>(consumerContext));
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                var scopeContext = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);

                serviceScope.SetCurrentConsumeContext(scopeContext);

                var consumer = scopeServiceProvider.GetService<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(scopeContext, consumer);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(
                    new CreatedConsumerScopeContext<IServiceScope, TConsumer, T>(serviceScope, consumerContext));
            }
            catch (Exception ex)
            {
                if (serviceScope is IAsyncDisposable asyncDisposable)
                    return ex.DisposeAsync<IConsumerScopeContext<TConsumer, T>>(() => asyncDisposable.DisposeAsync());

                serviceScope.Dispose();
                throw;
            }
        }
    }
}
