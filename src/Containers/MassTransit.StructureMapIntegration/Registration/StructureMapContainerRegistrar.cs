namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using Automatonymous;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;
    using StructureMap;


    public class StructureMapContainerRegistrar :
        IContainerRegistrar
    {
        readonly ConfigurationExpression _expression;

        public StructureMapContainerRegistrar(ConfigurationExpression expression)
        {
            _expression = expression;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _expression.ForConcreteType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _expression.For<IConsumerDefinition<TConsumer>>()
                .Use<TDefinition>();
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _expression.For<ISagaStateMachineFactory>().Use<StructureMapSagaStateMachineFactory>().Singleton();
            _expression.For<IStateMachineActivityFactory>().Use<StructureMapStateMachineActivityFactory>().Singleton();

            _expression.For<TStateMachine>().Singleton();
            _expression.For<SagaStateMachine<TInstance>>().Use(provider => provider.GetInstance<TStateMachine>()).Singleton();
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _expression.For<ISagaDefinition<TSaga>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _expression.ForConcreteType<TActivity>();

            _expression.For<IExecuteActivityScopeProvider<TActivity, TArguments>>()
                .Use(context => CreateExecuteActivityScopeProvider<TActivity, TArguments>(context));
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _expression.For<IActivityDefinition<TActivity, TArguments, TLog>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _expression.For<IExecuteActivityDefinition<TActivity, TArguments>>()
                .Use<TDefinition>();
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _expression.For<IEndpointDefinition<T>>().Use<TDefinition>();

            if (settings != null)
                _expression.ForSingletonOf<IEndpointSettings<IEndpointDefinition<T>>>().Use(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _expression.For<IRequestClient<T>>().Use(context => CreateRequestClient<T>(timeout, context));
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _expression.For<IRequestClient<T>>().Use(context => CreateRequestClient<T>(destinationAddress, timeout, context));
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _expression.ForConcreteType<TActivity>();

            _expression.For<ICompensateActivityScopeProvider<TActivity, TLog>>()
                .Use(context => CreateCompensateActivityScopeProvider<TActivity, TLog>(context));
        }

        static IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout, IContext context)
            where T : class
        {
            var clientFactory = context.GetInstance<IClientFactory>();

            var consumeContext = context.TryGetInstance<ConsumeContext>();
            return consumeContext != null
                ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                : clientFactory.CreateRequestClient<T>(timeout);
        }

        static IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout, IContext context)
            where T : class
        {
            var clientFactory = context.GetInstance<IClientFactory>();

            var consumeContext = context.TryGetInstance<ConsumeContext>();
            return consumeContext != null
                ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IContext context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return new StructureMapExecuteActivityScopeProvider<TActivity, TArguments>(context.GetInstance<IContainer>());
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IContext context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return new StructureMapCompensateActivityScopeProvider<TActivity, TLog>(context.GetInstance<IContainer>());
        }
    }
}
