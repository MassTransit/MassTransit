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
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

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

        /// <summary>
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory"/> from the container.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRequestClient<T> CreateRequestClient<T>(this IServiceProvider provider, RequestTimeout timeout = default)
            where T : class
        {
            return provider.GetRequiredService<IClientFactory>().CreateRequestClient<T>(timeout);
        }

        /// <summary>
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory"/> from the container.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRequestClient<T> CreateRequestClient<T>(this IServiceProvider provider, Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return provider.GetRequiredService<IClientFactory>().CreateRequestClient<T>(destinationAddress, timeout);
        }
    }
}
