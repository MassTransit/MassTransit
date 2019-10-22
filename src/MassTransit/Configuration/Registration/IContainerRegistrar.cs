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

        void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance;

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
    }
}
