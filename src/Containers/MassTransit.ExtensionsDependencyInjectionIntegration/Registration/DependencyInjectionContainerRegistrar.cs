namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Automatonymous;
    using Clients;
    using Definition;
    using MassTransit.Registration;
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
            _collection.AddTransient<IConsumerDefinition<TConsumer>, TDefinition>();
        }

        void IContainerRegistrar.RegisterSaga<T>()
        {
        }

        void IContainerRegistrar.RegisterSagaStateMachine<TStateMachine, TInstance>()
        {
            _collection.TryAddSingleton<IStateMachineActivityFactory, DependencyInjectionStateMachineActivityFactory>();
            _collection.TryAddSingleton<ISagaStateMachineFactory, DependencyInjectionSagaStateMachineFactory>();

            _collection.AddSingleton<TStateMachine>();
            _collection.AddSingleton<SagaStateMachine<TInstance>>(provider => provider.GetRequiredService<TStateMachine>());
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
        {
            _collection.AddSingleton(provider =>
            {
                var configurationServiceProvider = provider.GetRequiredService<IConfigurationServiceProvider>();

                return repositoryFactory(configurationServiceProvider);
            });
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
            _collection.AddTransient<ISagaDefinition<TSaga>, TDefinition>();
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
            _collection.AddTransient<IActivityDefinition<TActivity, TArguments, TLog>, TDefinition>();
        }

        void IContainerRegistrar.RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
        {
            _collection.AddTransient<IExecuteActivityDefinition<TActivity, TArguments>, TDefinition>();
        }

        void IContainerRegistrar.RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
        {
            _collection.AddTransient<IEndpointDefinition<T>, TDefinition>();

            if (settings != null)
                _collection.AddSingleton(settings);
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
            _collection.TryAddScoped(provider => factoryMethod(provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        void IContainerRegistrar.RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
        {
            _collection.TryAddSingleton(provider => factoryMethod(provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        void IContainerRegistrar.RegisterSingleInstance<T>(T instance)
        {
            _collection.TryAddSingleton(instance);
        }

        protected virtual IClientFactory GetClientFactory(IServiceProvider provider)
        {
            return provider.GetRequiredService<IClientFactory>();
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
}
