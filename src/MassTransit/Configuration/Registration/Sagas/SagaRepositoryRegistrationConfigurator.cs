namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Definition;
    using Futures;
    using Saga;


    public class SagaRepositoryRegistrationConfigurator<TSaga> :
        ISagaRepositoryRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainerRegistrar _registrar;

        public SagaRepositoryRegistrationConfigurator(IContainerRegistrar registrar)
        {
            _registrar = registrar;
        }

        void IContainerRegistrar.RegisterConsumer<T>()
        {
            _registrar.RegisterConsumer<T>();
        }

        void IContainerRegistrar.RegisterConsumerDefinition<TDefinition, TConsumer>()
        {
            _registrar.RegisterConsumerDefinition<TDefinition, TConsumer>();
        }

        void IContainerRegistrar.RegisterSaga<T>()
        {
            _registrar.RegisterSaga<T>();
        }

        void IContainerRegistrar.RegisterSagaStateMachine<TStateMachine, TInstance>()
        {
            _registrar.RegisterSagaStateMachine<TStateMachine, TInstance>();
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga1>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga1>> repositoryFactory)
        {
            _registrar.RegisterSagaRepository(repositoryFactory);
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga1, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _registrar.RegisterSagaRepository<TSaga1, TContext, TConsumeContextFactory, TRepositoryContextFactory>();
        }

        void IContainerRegistrar.RegisterSagaDefinition<TDefinition, TSaga1>()
        {
            _registrar.RegisterSagaDefinition<TDefinition, TSaga1>();
        }

        void IContainerRegistrar.RegisterExecuteActivity<TActivity, TArguments>()
        {
            _registrar.RegisterExecuteActivity<TActivity, TArguments>();
        }

        void IContainerRegistrar.RegisterCompensateActivity<TActivity, TLog>()
        {
            _registrar.RegisterCompensateActivity<TActivity, TLog>();
        }

        void IContainerRegistrar.RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
        {
            _registrar.RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>();
        }

        void IContainerRegistrar.RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
        {
            _registrar.RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>();
        }

        void IContainerRegistrar.RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
        {
            _registrar.RegisterEndpointDefinition<TDefinition, T>(settings);
        }

        public void RegisterFuture<TFuture>()
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _registrar.RegisterFuture<TFuture>();
        }

        public void RegisterFutureDefinition<TDefinition, TFuture>()
            where TDefinition : class, IFutureDefinition<TFuture>
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _registrar.RegisterFutureDefinition<TDefinition, TFuture>();
        }

        void IContainerRegistrar.RegisterRequestClient<T>(RequestTimeout timeout)
        {
            _registrar.RegisterRequestClient<T>(timeout);
        }

        void IContainerRegistrar.RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
        {
            _registrar.RegisterRequestClient<T>(destinationAddress, timeout);
        }

        void IContainerRegistrar.Register<T, TImplementation>()
        {
            _registrar.Register<T, TImplementation>();
        }

        void IContainerRegistrar.Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
        {
            _registrar.Register(factoryMethod);
        }

        void IContainerRegistrar.RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
        {
            _registrar.RegisterSingleInstance(factoryMethod);
        }

        void IContainerRegistrar.RegisterSingleInstance<T>(T instance)
        {
            _registrar.RegisterSingleInstance(instance);
        }
    }
}
