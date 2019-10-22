namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Automatonymous;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
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

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            if (!_container.Kernel.HasComponent(typeof(IStateMachineActivityFactory)))
                _container.Register(Component.For<IStateMachineActivityFactory>().ImplementedBy<WindsorStateMachineActivityFactory>().LifestyleSingleton());

            if (!_container.Kernel.HasComponent(typeof(ISagaStateMachineFactory)))
                _container.Register(Component.For<ISagaStateMachineFactory>().ImplementedBy<WindsorSagaStateMachineFactory>().LifestyleSingleton());

            _container.Register(
                Component.For<TStateMachine>().LifestyleSingleton(),
                Component.For<SagaStateMachine<TInstance>>().UsingFactoryMethod(provider => provider.Resolve<TStateMachine>()).LifestyleSingleton()
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

                var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
                return currentScope != null
                    ? clientFactory.CreateRequestClient<T>(kernel.Resolve<ConsumeContext>(), timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            }));
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _container.Register(Component.For<IRequestClient<T>>().UsingFactoryMethod(kernel =>
            {
                var clientFactory = kernel.Resolve<IClientFactory>();

                var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
                return currentScope != null
                    ? clientFactory.CreateRequestClient<T>(kernel.Resolve<ConsumeContext>(), destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            }));
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

        void RegisterActivityIfNotPresent<TActivity>()
            where TActivity : class
        {
            if (!_container.Kernel.HasComponent(typeof(TActivity)))
                _container.Register(Component.For<TActivity>().LifestyleScoped());
        }
    }
}
