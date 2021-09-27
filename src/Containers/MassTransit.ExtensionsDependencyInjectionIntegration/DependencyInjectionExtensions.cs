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

        /// <summary>
        /// Registers a generic request client provider in the container, which will be used for any
        /// client that is not explicitly registered using AddRequestClient.
        /// </summary>
        /// <param name="collection"></param>
        public static IServiceCollection AddGenericRequestClient(this IServiceCollection collection)
        {
            collection.AddScoped(typeof(IRequestClient<>), typeof(GenericRequestClient<>));

            return collection;
        }


        class GenericRequestClient<TRequest> :
            IRequestClient<TRequest>
            where TRequest : class
        {
            readonly IRequestClient<TRequest> _client;

            public GenericRequestClient(IServiceProvider provider)
            {
                var clientFactory = provider.GetService<IClientFactory>() ?? provider.GetService<IMediator>();
                if (clientFactory == null)
                    throw new MassTransitException($"Unable to resolve bus or mediator for request client: {TypeCache<TRequest>.ShortName}");

                var consumeContext = provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

                _client = consumeContext != null
                    ? clientFactory.CreateRequestClient<TRequest>(consumeContext)
                    : new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(clientFactory, provider))
                        .CreateRequestClient<TRequest>(default);
            }

            public RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
            {
                return _client.Create(message, cancellationToken, timeout);
            }

            public RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken, RequestTimeout timeout)
            {
                return _client.Create(values, cancellationToken, timeout);
            }

            public Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
                where T : class
            {
                return _client.GetResponse<T>(message, cancellationToken, timeout);
            }

            public Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken, RequestTimeout timeout)
                where T : class
            {
                return _client.GetResponse<T>(message, callback, cancellationToken, timeout);
            }

            public Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
                where T : class
            {
                return _client.GetResponse<T>(values, cancellationToken, timeout);
            }

            public Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken, RequestTimeout timeout)
                where T : class
            {
                return _client.GetResponse<T>(values, callback, cancellationToken, timeout);
            }

            public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(message, cancellationToken, timeout);
            }

            public Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken,
                RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(message, callback, cancellationToken, timeout);
            }

            public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(values, cancellationToken, timeout);
            }

            public Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
            {
                return _client.GetResponse<T1, T2>(values, callback, cancellationToken, timeout);
            }

            public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken,
                RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(message, cancellationToken, timeout);
            }

            public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(message, callback, cancellationToken, timeout);
            }

            public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken,
                RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(values, cancellationToken, timeout);
            }

            public Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
                CancellationToken cancellationToken, RequestTimeout timeout)
                where T1 : class
                where T2 : class
                where T3 : class
            {
                return _client.GetResponse<T1, T2, T3>(values, callback, cancellationToken, timeout);
            }
        }
    }
}
