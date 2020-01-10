namespace MassTransit
{
    using System;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Pipeline.Filters;
    using Saga;
    using Scoping;


    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Creates a scope which is used by all downstream filters/consumers/etc.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
        public static void UseServiceScope(this IPipeConfigurator<ConsumeContext> configurator, IServiceProvider serviceProvider)
        {
            var scopeProvider = new DependencyInjectionConsumerScopeProvider(serviceProvider);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this IServiceCollection collection)
            where T : class, ISaga
        {
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            registrar.RegisterInMemorySagaRepository<T>();
        }

        public static ConsumeContext GetConsumeContext(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();
        }
    }
}
