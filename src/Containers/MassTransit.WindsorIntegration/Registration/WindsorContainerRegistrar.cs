namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Automatonymous;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Clients;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class WindsorContainerRegistrar :
        IContainerRegistrar
    {
        readonly IWindsorContainer _container;

        public WindsorContainerRegistrar(IWindsorContainer container)
        {
            _container = container;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _container.Register(
                Component.For<T>()
                    .LifestyleScoped());
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _container.Register(
                Component.For<IConsumerDefinition<TConsumer>>()
                    .ImplementedBy<TDefinition>());
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterSagaStateMachine<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            if (!_container.Kernel.HasComponent(typeof(IStateMachineActivityFactory)))
                _container.Register(Component.For<IStateMachineActivityFactory>().ImplementedBy<WindsorStateMachineActivityFactory>().LifestyleScoped());

            if (!_container.Kernel.HasComponent(typeof(ISagaStateMachineFactory)))
                _container.Register(Component.For<ISagaStateMachineFactory>().ImplementedBy<WindsorSagaStateMachineFactory>().LifestyleSingleton());

            _container.Register(
                Component.For<TStateMachine>().LifestyleSingleton(),
                Component.For<SagaStateMachine<TInstance>>().UsingFactoryMethod(provider => provider.Resolve<TStateMachine>()).LifestyleSingleton()
            );
        }

        public void RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
            where TSaga : class, ISaga
        {
            _container.Register(Component.For<ISagaRepository<TSaga>>().UsingFactoryMethod(provider =>
                repositoryFactory(provider.Resolve<IConfigurationServiceProvider>())).LifestyleSingleton());
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _container.Register(
                Component.For<ISagaConsumeContextFactory<TContext, TSaga>, TConsumeContextFactory>().LifestyleScoped(),
                Component.For<ISagaRepositoryContextFactory<TSaga>, TRepositoryContextFactory>().LifestyleScoped(),
                Component.For<WindsorSagaRepositoryContextFactory<TSaga>>().LifestyleSingleton(),
                Component.For<ISagaRepository<TSaga>>().UsingFactoryMethod(provider =>
                    new SagaRepository<TSaga>(provider.Resolve<WindsorSagaRepositoryContextFactory<TSaga>>())).LifestyleSingleton()
            );
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _container.Register(
                Component.For<ISagaDefinition<TSaga>>()
                    .ImplementedBy<TDefinition>());
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            RegisterActivityIfNotPresent<TActivity>();

            _container.Register(
                Component.For<IExecuteActivityScopeProvider<TActivity, TArguments>>()
                    .ImplementedBy<WindsorExecuteActivityScopeProvider<TActivity, TArguments>>());
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            RegisterActivityIfNotPresent<TActivity>();

            _container.Register(
                Component.For<ICompensateActivityScopeProvider<TActivity, TLog>>()
                    .ImplementedBy<WindsorCompensateActivityScopeProvider<TActivity, TLog>>());
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _container.Register(
                Component.For<IActivityDefinition<TActivity, TArguments, TLog>>()
                    .ImplementedBy<TDefinition>());
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _container.Register(
                Component.For<IExecuteActivityDefinition<TActivity, TArguments>>()
                    .ImplementedBy<TDefinition>());
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _container.Register(
                Component.For<IEndpointDefinition<T>>()
                    .ImplementedBy<TDefinition>(),
                Component.For<IEndpointSettings<IEndpointDefinition<T>>>().Instance(settings));
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _container.Register(Component.For<IRequestClient<T>>().UsingFactoryMethod(kernel =>
            {
                var clientFactory = kernel.Resolve<IClientFactory>();
                var consumeContext = kernel.GetConsumeContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<IKernel>(clientFactory, kernel))
                    .CreateRequestClient<T>(timeout);
            }));
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _container.Register(Component.For<IRequestClient<T>>().UsingFactoryMethod(kernel =>
            {
                var clientFactory = kernel.Resolve<IClientFactory>();
                var consumeContext = kernel.GetConsumeContext();

                if (consumeContext != null)
                    return clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<IKernel>(clientFactory, kernel))
                    .CreateRequestClient<T>(destinationAddress, timeout);
            }));
        }

        public void Register<T, TImplementation>()
            where T : class
            where TImplementation : class, T
        {
            if (!_container.Kernel.HasComponent(typeof(T)))
                _container.Register(Component.For<T>().ImplementedBy<TImplementation>().LifestyleScoped());
        }

        public void Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _container.Register(Component.For<T>().UsingFactoryMethod(kernel => factoryMethod(kernel.Resolve<IConfigurationServiceProvider>()))
                .LifestyleScoped());
        }

        public void RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            if (!_container.Kernel.HasComponent(typeof(T)))
                _container.Register(Component.For<T>().UsingFactoryMethod(kernel => factoryMethod(kernel.Resolve<IConfigurationServiceProvider>()))
                    .LifestyleSingleton());
        }

        public void RegisterSingleInstance<T>(T instance)
            where T : class
        {
            if (!_container.Kernel.HasComponent(typeof(T)))
                _container.Register(Component.For<T>().Instance(instance).LifestyleSingleton());
        }

        void RegisterActivityIfNotPresent<TActivity>()
            where TActivity : class
        {
            if (!_container.Kernel.HasComponent(typeof(TActivity)))
                _container.Register(Component.For<TActivity>().LifestyleScoped());
        }
    }
}
