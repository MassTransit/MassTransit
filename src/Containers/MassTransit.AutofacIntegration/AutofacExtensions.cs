namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Builder;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using AutofacIntegration.ScopeProviders;
    using Clients;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Specifications;
    using Mediator;
    using Pipeline.Filters;
    using Scoping;


    public static class AutofacExtensions
    {
        /// <summary>
        /// Creates a lifetime scope which is shared by any downstream filters (rather than creating a new one).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="name">The name of the lifetime scope</param>
        /// <param name="configureScope">Configuration for scope container</param>
        public static void UseLifetimeScope(this IPipeConfigurator<ConsumeContext> configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            var scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(lifetimeScope), name, configureScope);
            var specification = new FilterPipeSpecification<ConsumeContext>(new ScopeConsumeFilter(scopeProvider));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Creates a service scope for each message type, compatible with UseMessageRetry and UseInMemoryOutbox
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="name">The name of the lifetime scope</param>
        /// <param name="configureScope">Configuration for scope container</param>
        public static void UseMessageLifetimeScope(this IConsumePipeConfigurator configurator, ILifetimeScope lifetimeScope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (lifetimeScope == null)
                throw new ArgumentNullException(nameof(lifetimeScope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var observer = new MessageLifetimeScopeConfigurationObserver(configurator, new SingleLifetimeScopeProvider(lifetimeScope), name, configureScope);
        }

        /// <summary>
        /// Register an accessor for an input type in the container
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="propertyExpression"></param>
        public static IRegistrationBuilder<ILifetimeScopeIdAccessor<TInput, T>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeIdAccessor<TInput, T>(this ContainerBuilder builder, Expression<Func<TInput, T>> propertyExpression)
            where TInput : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var propertyInfo = propertyExpression.GetPropertyInfo();

            return builder.RegisterType<MessageLifetimeScopeIdAccessor<TInput, T>>()
                .As<ILifetimeScopeIdAccessor<TInput, T>>()
                .WithParameter(TypedParameter.From(propertyInfo));
        }

        /// <summary>
        /// Register a lifetime scope registry for the given identifier type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="scopeTag"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<ILifetimeScopeRegistry<string>, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterLifetimeScopeRegistry<T>(this ContainerBuilder builder, object scopeTag)
        {
            return builder.RegisterType<LifetimeScopeRegistry<string>>()
                .As<ILifetimeScopeRegistry<string>>()
                .WithParameter("tag", scopeTag)
                .SingleInstance();
        }

        /// <summary>
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory" /> from the container.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRequestClient<T> CreateRequestClient<T>(this ILifetimeScope scope, RequestTimeout timeout = default)
            where T : class
        {
            return scope.Resolve<IClientFactory>().CreateRequestClient<T>(timeout);
        }

        /// <summary>
        /// Create a request client, using the specified service address, using the <see cref="IClientFactory" /> from the container.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRequestClient<T> CreateRequestClient<T>(this ILifetimeScope scope, Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return scope.Resolve<IClientFactory>().CreateRequestClient<T>(destinationAddress, timeout);
        }

        /// <summary>
        /// Registers a generic request client provider in the container, which will be used for any
        /// client that is not explicitly registered using AddRequestClient.
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterGenericRequestClient(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRequestClient<>)).As(typeof(IRequestClient<>)).InstancePerLifetimeScope();
        }


        class GenericRequestClient<TRequest> :
            IRequestClient<TRequest>
            where TRequest : class
        {
            readonly IRequestClient<TRequest> _client;

            public GenericRequestClient(ILifetimeScope scope)
            {
                var clientFactory = scope.ResolveOptional<IClientFactory>() ?? scope.ResolveOptional<IMediator>();
                if (clientFactory == null)
                    throw new MassTransitException($"Unable to resolve bus or mediator for request client: {TypeCache<TRequest>.ShortName}");

                _client = scope.TryResolve(out ConsumeContext consumeContext)
                    ? clientFactory.CreateRequestClient<TRequest>(consumeContext)
                    : new ClientFactory(new ScopedClientFactoryContext<ILifetimeScope>(clientFactory, scope))
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
