namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Automatonymous;
    using Clients;
    using Conductor.Directory;
    using Definition;
    using Futures;
    using Internals.Extensions;
    using MassTransit.Registration;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class DependencyInjectionContainerRegistrar :
        IContainerRegistrar
    {
        readonly IServiceCollection _collection;

        public DependencyInjectionContainerRegistrar(IServiceCollection collection)
        {
            _collection = collection;
        }

        void IContainerRegistrar.RegisterConsumer<T>()
        {
            _collection.AddScoped<T>();
        }

        void IContainerRegistrar.RegisterConsumerDefinition<TDefinition, TConsumer>()
        {
            _collection.AddSingleton<TDefinition>();
            _collection.AddSingleton<IConsumerDefinition<TConsumer>>(provider => provider.GetRequiredService<TDefinition>());

            ConfigureServiceDirectoryIfPresent<TDefinition>();
        }

        void IContainerRegistrar.RegisterSaga<T>()
        {
        }

        void IContainerRegistrar.RegisterSagaStateMachine<TStateMachine, TInstance>()
        {
            _collection.AddSingleton<TStateMachine>();
            _collection.AddSingleton<SagaStateMachine<TInstance>>(provider => provider.GetRequiredService<TStateMachine>());
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
        {
            RegisterSingleInstance(provider => repositoryFactory(provider));
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _collection.AddScoped<ISagaConsumeContextFactory<TContext, TSaga>, TConsumeContextFactory>();
            _collection.AddScoped<ISagaRepositoryContextFactory<TSaga>, TRepositoryContextFactory>();

            _collection.AddSingleton<DependencyInjectionSagaRepositoryContextFactory<TSaga>>();
            _collection.AddSingleton<ISagaRepository<TSaga>>(provider =>
                new SagaRepository<TSaga>(provider.GetRequiredService<DependencyInjectionSagaRepositoryContextFactory<TSaga>>()));
        }

        void IContainerRegistrar.RegisterSagaDefinition<TDefinition, TSaga>()
        {
            _collection.AddSingleton<TDefinition>();
            _collection.AddSingleton<ISagaDefinition<TSaga>>(provider => provider.GetRequiredService<TDefinition>());

            ConfigureServiceDirectoryIfPresent<TDefinition>();
        }

        void IContainerRegistrar.RegisterExecuteActivity<TActivity, TArguments>()
        {
            _collection.TryAddScoped<TActivity>();

            _collection.AddTransient<IExecuteActivityScopeProvider<TActivity, TArguments>,
                DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>>();
        }

        void IContainerRegistrar.RegisterCompensateActivity<TActivity, TLog>()
        {
            _collection.TryAddScoped<TActivity>();

            _collection.AddTransient<ICompensateActivityScopeProvider<TActivity, TLog>, DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog>>();
        }

        void IContainerRegistrar.RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
        {
            _collection.AddSingleton<TDefinition>();
            _collection.AddSingleton<IActivityDefinition<TActivity, TArguments, TLog>>(provider => provider.GetRequiredService<TDefinition>());

            ConfigureServiceDirectoryIfPresent<TDefinition>();
        }

        void IContainerRegistrar.RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
        {
            _collection.AddSingleton<TDefinition>();
            _collection.AddSingleton<IExecuteActivityDefinition<TActivity, TArguments>>(provider => provider.GetRequiredService<TDefinition>());

            ConfigureServiceDirectoryIfPresent<TDefinition>();
        }

        void IContainerRegistrar.RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
        {
            _collection.AddTransient<IEndpointDefinition<T>, TDefinition>();

            if (settings != null)
                _collection.AddSingleton(settings);
        }

        public void RegisterFuture<TFuture>()
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _collection.AddSingleton<TFuture>();
        }

        public void RegisterFutureDefinition<TDefinition, TFuture>()
            where TDefinition : class, IFutureDefinition<TFuture>
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _collection.AddSingleton<TDefinition>();
            _collection.AddSingleton<IFutureDefinition<TFuture>>(provider => provider.GetRequiredService<TDefinition>());

            ConfigureServiceDirectoryIfPresent<TDefinition>();
        }

        void IContainerRegistrar.RegisterRequestClient<T>(RequestTimeout timeout)
        {
            _collection.AddScoped(provider =>
            {
                var clientFactory = GetClientFactory(provider);
                var consumeContext = provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(clientFactory, provider))
                    .CreateRequestClient<T>(timeout);
            });
        }

        void IContainerRegistrar.RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
        {
            _collection.AddScoped(provider =>
            {
                var clientFactory = GetClientFactory(provider);
                var consumeContext = provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(clientFactory, provider))
                    .CreateRequestClient<T>(destinationAddress, timeout);
            });
        }

        public void Register<T, TImplementation>()
            where T : class
            where TImplementation : class, T
        {
            _collection.TryAddScoped<T, TImplementation>();
        }

        public void Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _collection.TryAddScoped(provider => factoryMethod(new DependencyInjectionConfigurationServiceProvider(provider)));
        }

        public void RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _collection.TryAddSingleton(provider => factoryMethod(new DependencyInjectionConfigurationServiceProvider(provider)));
        }

        public void RegisterSingleInstance<T>(T instance)
            where T : class
        {
            _collection.TryAddSingleton(instance);
        }

        protected virtual IClientFactory GetClientFactory(IServiceProvider provider)
        {
            return provider.GetRequiredService<IClientFactory>();
        }

        void ConfigureServiceDirectoryIfPresent<TDefinition>()
        {
            if (typeof(TDefinition).HasInterface<IConfigureServiceDirectory>())
                _collection.AddSingleton(provider => (IConfigureServiceDirectory)provider.GetRequiredService<TDefinition>());
        }
    }


    public class DependencyInjectionContainerRegistrar<TBus> :
        DependencyInjectionContainerRegistrar
    {
        public DependencyInjectionContainerRegistrar(IServiceCollection collection)
            : base(collection)
        {
        }

        protected override IClientFactory GetClientFactory(IServiceProvider provider)
        {
            return provider.GetRequiredService<Bind<TBus, IClientFactory>>().Value;
        }
    }


    public class DependencyInjectionMediatorContainerRegistrar :
        DependencyInjectionContainerRegistrar
    {
        public DependencyInjectionMediatorContainerRegistrar(IServiceCollection collection)
            : base(collection)
        {
        }

        protected override IClientFactory GetClientFactory(IServiceProvider provider)
        {
            return provider.GetRequiredService<IMediator>();
        }
    }
}
