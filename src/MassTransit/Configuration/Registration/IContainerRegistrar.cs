namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Courier;
    using Definition;
    using Saga;


    public interface IContainerRegistrar
    {
        void RegisterConsumer<T>()
            where T : class, IConsumer;

        void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer;

        void RegisterSaga<T>()
            where T : class, ISaga;

        void RegisterSagaStateMachine<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance;

        void RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
            where TSaga : class, ISaga;

        void RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
            where TSaga : class, ISaga
            where TContext : class
            where TConsumeContextFactory : class, ISagaConsumeContextFactory<TContext, TSaga>
            where TRepositoryContextFactory : class, ISagaRepositoryContextFactory<TSaga>;

        void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga;

        void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class;

        void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class;

        void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings = null)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class;

        void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Register a service, implemented by the implementation type, which is created in each
        /// container scope.
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        void Register<T, TImplementation>()
            where T : class
            where TImplementation : class, T;

        /// <summary>
        /// Register a service, implemented by the implementation type, which is created in each
        /// container scope.
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <param name="factoryMethod">Creates the instance, when it is first accessed by the container</param>
        void Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class;

        /// <summary>
        /// Register a single instance, which is created by the specified factory method
        /// </summary>
        /// <param name="factoryMethod">Creates the instance, when it is first accessed by the container</param>
        /// <typeparam name="T">The interface type</typeparam>
        void RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class;

        /// <summary>
        /// Register a single instance, which is specified
        /// </summary>
        /// <param name="instance">The service type instance</param>
        /// <typeparam name="T"></typeparam>
        void RegisterSingleInstance<T>(T instance)
            where T : class;
    }
}
