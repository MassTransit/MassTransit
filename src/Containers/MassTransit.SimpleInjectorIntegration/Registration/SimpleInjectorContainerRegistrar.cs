namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Clients;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;


    public class SimpleInjectorContainerRegistrar :
        IContainerRegistrar
    {
        readonly Container _container;
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorContainerRegistrar(Container container)
        {
            _container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(_container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _container.Register<T>(Lifestyle.Scoped);
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _container.Register<IConsumerDefinition<TConsumer>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterSagaStateMachine<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _container.RegisterSingleton<IStateMachineActivityFactory, SimpleInjectorStateMachineActivityFactory>();
            _container.RegisterSingleton<ISagaStateMachineFactory, SimpleInjectorSagaStateMachineFactory>();

            _container.RegisterSingleton<TStateMachine>();
            _container.RegisterSingleton<SagaStateMachine<TInstance>>(() => _container.GetInstance<TStateMachine>());
        }

        public void RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
            where TSaga : class, ISaga
        {
            _container.RegisterSingleton(() =>
            {
                var configurationServiceProvider = _container.GetInstance<IConfigurationServiceProvider>();

                return repositoryFactory(configurationServiceProvider);
            });
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _container.Register<ISagaConsumeContextFactory<TContext, TSaga>, TConsumeContextFactory>(Lifestyle.Scoped);
            _container.Register<ISagaRepositoryContextFactory<TSaga>, TRepositoryContextFactory>(Lifestyle.Scoped);

            _container.RegisterSingleton<SimpleInjectorSagaRepositoryContextFactory<TSaga>>();
            _container.RegisterSingleton<ISagaRepository<TSaga>>(() =>
                new SagaRepository<TSaga>(_container.GetInstance<SimpleInjectorSagaRepositoryContextFactory<TSaga>>()));
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _container.Register<ISagaDefinition<TSaga>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            RegisterIfNotRegistered<TActivity>();

            _container.Register<IExecuteActivityScopeProvider<TActivity, TArguments>,
                SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>>(Lifestyle.Transient);
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            RegisterIfNotRegistered<TActivity>();

            _container.Register<ICompensateActivityScopeProvider<TActivity, TLog>,
                SimpleInjectorCompensateActivityScopeProvider<TActivity, TLog>>(Lifestyle.Transient);
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _container.Register<IActivityDefinition<TActivity, TArguments, TLog>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _container.Register<IExecuteActivityDefinition<TActivity, TArguments>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _container.Register<IEndpointDefinition<T>, TDefinition>(Lifestyle.Transient);

            if (settings != null)
                _container.RegisterInstance(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            _container.Register(() =>
            {
                var clientFactory = _container.GetInstance<IClientFactory>();
                var consumeContext = _container.GetConsumeContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<Container>(clientFactory, _container))
                    .CreateRequestClient<T>(timeout);
            }, _hybridLifestyle);
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            _container.Register(() =>
            {
                var clientFactory = _container.GetInstance<IClientFactory>();
                var consumeContext = _container.GetConsumeContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<Container>(clientFactory, _container))
                    .CreateRequestClient<T>(destinationAddress, timeout);
            }, _hybridLifestyle);
        }

        public void Register<T, TImplementation>()
            where T : class
            where TImplementation : class, T
        {
            _container.Register<T, TImplementation>(Lifestyle.Scoped);
        }

        public void Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _container.Register(() => factoryMethod(_container.GetInstance<IConfigurationServiceProvider>()));
        }

        public void RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _container.RegisterSingleton(() => factoryMethod(_container.GetInstance<IConfigurationServiceProvider>()));
        }

        public void RegisterSingleInstance<T>(T instance)
            where T : class
        {
            _container.RegisterInstance(instance);
        }

        void RegisterIfNotRegistered<TActivity>()
            where TActivity : class
        {
            var notExists = _container.GetCurrentRegistrations().SingleOrDefault(r => r.Registration.ImplementationType == typeof(TActivity)) == null;

            if (notExists)
                _container.Register<TActivity>(Lifestyle.Scoped);
        }
    }
}
