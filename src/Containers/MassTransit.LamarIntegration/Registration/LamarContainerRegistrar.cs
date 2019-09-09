namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Automatonymous;
    using Courier;
    using Definition;
    using Lamar;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class LamarContainerRegistrar :
        IContainerRegistrar
    {
        readonly ServiceRegistry _registry;

        public LamarContainerRegistrar(ServiceRegistry registry)
        {
            _registry = registry;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _registry.ForConcreteType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _registry.For<IConsumerDefinition<TConsumer>>()
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
            _registry.TryAddSingleton<IStateMachineActivityFactory, LamarStateMachineActivityFactory>();
            _registry.TryAddSingleton<ISagaStateMachineFactory, LamarSagaStateMachineFactory>();

            _registry.AddSingleton<TStateMachine>();
            _registry.AddSingleton<SagaStateMachine<TInstance>>(provider => provider.GetRequiredService<TStateMachine>());
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _registry.For<ISagaDefinition<TSaga>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _registry.ForConcreteType<TActivity>();

            _registry.For<IExecuteActivityScopeProvider<TActivity, TArguments>>()
                .Use(CreateExecuteActivityScopeProvider<TActivity, TArguments>);
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _registry.For<IActivityDefinition<TActivity, TArguments, TLog>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _registry.For<IExecuteActivityDefinition<TActivity, TArguments>>()
                .Use<TDefinition>();
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _registry.For<IEndpointDefinition<T>>().Use<TDefinition>();

            if (settings != null)
                _registry.ForSingletonOf<IEndpointSettings<IEndpointDefinition<T>>>().Use(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                var consumeContext = context.TryGetInstance<ConsumeContext>();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            }).Scoped();
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                var consumeContext = context.TryGetInstance<ConsumeContext>();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            }).Scoped();
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _registry.ForConcreteType<TActivity>();

            _registry.For<ICompensateActivityScopeProvider<TActivity, TLog>>()
                .Use(CreateCompensateActivityScopeProvider<TActivity, TLog>);
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IServiceContext context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return new LamarExecuteActivityScopeProvider<TActivity, TArguments>(context.GetRequiredService<IContainer>());
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IServiceContext context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return new LamarCompensateActivityScopeProvider<TActivity, TLog>(context.GetRequiredService<IContainer>());
        }
    }
}
