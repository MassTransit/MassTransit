namespace MassTransit
{
    using System;
    using Configuration;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middleware;
    using Saga;


    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Creates a single scope for the receive endpoint that is used by all consumers, sagas, messages, etc.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void UseServiceScope(this IConsumePipeConfigurator configurator, IRegistrationContext context)
        {
            var scopeProvider = new ConsumeScopeProvider(context);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

            configurator.AddPrePipeSpecification(specification);
        }

        /// <summary>
        /// Creates a single scope for the receive endpoint that is used by all consumers, sagas, messages, etc.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
        [Obsolete("Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.")]
        public static void UseServiceScope(this IConsumePipeConfigurator configurator, IServiceProvider serviceProvider)
        {
            var scopeProvider = new ConsumeScopeProvider(serviceProvider, LegacySetScopedConsumeContext.Instance);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

            configurator.AddPrePipeSpecification(specification);
        }

        /// <summary>
        /// Creates a scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void UseMessageScope(this IConsumePipeConfigurator configurator, IRegistrationContext context)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var observer = new MessageScopeConfigurationObserver(configurator, context);
        }

        /// <summary>
        /// Creates a scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
        [Obsolete("Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.")]
        public static void UseMessageScope(this IConsumePipeConfigurator configurator, IServiceProvider serviceProvider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var observer = new MessageScopeConfigurationObserver(configurator, serviceProvider);
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this IServiceCollection collection)
            where T : class, ISaga
        {
            collection.TryAddSingleton(new IndexedSagaDictionary<T>());
            collection.RegisterLoadSagaRepository<T, InMemorySagaRepositoryContextFactory<T>>();
            collection.RegisterQuerySagaRepository<T, InMemorySagaRepositoryContextFactory<T>>();
            collection.RegisterSagaRepository<T, IndexedSagaDictionary<T>, InMemorySagaConsumeContextFactory<T>, InMemorySagaRepositoryContextFactory<T>>();
        }

        /// <summary>
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory" /> from the container.
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
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory" /> from the container.
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

        /// <summary>
        /// Registers a generic request client provider in the container, which will be used for any
        /// client that is not explicitly registered using AddRequestClient.
        /// </summary>
        /// <param name="collection"></param>
        [Obsolete("Remove, the generic request client is automatically registered. Visit https://masstransit.io/obsolete for details.")]
        public static IServiceCollection AddGenericRequestClient(this IServiceCollection collection)
        {
            return collection;
        }
    }
}
