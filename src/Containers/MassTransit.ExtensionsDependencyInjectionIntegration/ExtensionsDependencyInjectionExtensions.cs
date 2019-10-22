namespace MassTransit
{
    using System;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Pipeline.Filters;
    using Saga;


    public static class ExtensionsDependencyInjectionIntegrationExtensions
    {
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            var registryConfigurator = serviceProvider.GetRequiredService<IRegistration>();

            registryConfigurator.ConfigureConsumers(configurator);
            registryConfigurator.ConfigureSagas(configurator);
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterInMemorySagaRepository(this IServiceCollection collection)
        {
            collection.AddSingleton(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this IServiceCollection collection)
            where T : class, ISaga
        {
            collection.AddSingleton<ISagaRepository<T>, InMemorySagaRepository<T>>();
        }

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
    }
}
