namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Specifications;
    using Mediator;
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
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Creates a service scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider"></param>
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
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            registrar.RegisterInMemorySagaRepository<T>();
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

        public static void AddGenericRequestClient(this IServiceCollection collection)
        {
            collection.AddScoped(typeof(IRequestClient<>), typeof(GenericRequestClient<>));
        }


        class GenericRequestClient<T> :
            IRequestClient<T>
            where T : class
        {
            readonly IRequestClient<T> _client;

            public GenericRequestClient(IServiceProvider provider)
            {
                var clientFactory = provider.GetService<IClientFactory>() ?? provider.GetService<IMediator>();
                if (clientFactory == null)
                    throw new MassTransitException($"Unable to resolve bus or mediator for request client: {TypeCache<T>.ShortName}");

                var consumeContext = provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

                _client = consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext)
                    : new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(clientFactory, provider))
                        .CreateRequestClient<T>(default);
            }

            RequestHandle<T> IRequestClient<T>.Create(T message, CancellationToken cancellationToken, RequestTimeout timeout)
            {
                return _client.Create(message, cancellationToken, timeout);
            }

            RequestHandle<T> IRequestClient<T>.Create(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            {
                return _client.Create(values, cancellationToken, timeout);
            }

            Task<Response<T1>> IRequestClient<T>.GetResponse<T1>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
            {
                return _client.GetResponse<T1>(message, cancellationToken, timeout);
            }

            Task<Response<T1>> IRequestClient<T>.GetResponse<T1>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
            {
                return _client.GetResponse<T1>(values, cancellationToken, timeout);
            }

            Task<Response<T1, T2>> IRequestClient<T>.GetResponse<T1, T2>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(message, cancellationToken, timeout);
            }

            Task<Response<T1, T2>> IRequestClient<T>.GetResponse<T1, T2>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(values, cancellationToken, timeout);
            }

            Task<Response<T1, T2, T3>> IRequestClient<T>.GetResponse<T1, T2, T3>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(message, cancellationToken, timeout);
            }

            Task<Response<T1, T2, T3>> IRequestClient<T>.GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(values, cancellationToken, timeout);
            }
        }
    }
}
