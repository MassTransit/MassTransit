namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using Automatonymous;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class AutofacContainerRegistrar :
        IContainerRegistrar
    {
        readonly ContainerBuilder _builder;

        public AutofacContainerRegistrar(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _builder.RegisterType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _builder.RegisterType<TDefinition>()
                .As<IConsumerDefinition<TConsumer>>();
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _builder.RegisterType<AutofacStateMachineActivityFactory>()
                .As<IStateMachineActivityFactory>()
                .SingleInstance();

            _builder.RegisterType<AutofacSagaStateMachineFactory>()
                .As<ISagaStateMachineFactory>()
                .SingleInstance();

            _builder.RegisterType<TStateMachine>()
                .AsSelf()
                .As<SagaStateMachine<TInstance>>()
                .SingleInstance();
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _builder.RegisterType<TDefinition>()
                .As<ISagaDefinition<TSaga>>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _builder.RegisterType<TActivity>();

            _builder.Register(CreateExecuteActivityScopeProvider<TActivity, TArguments>)
                .As<IExecuteActivityScopeProvider<TActivity, TArguments>>();
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _builder.RegisterType<TDefinition>()
                .As<IActivityDefinition<TActivity, TArguments, TLog>>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _builder.RegisterType<TDefinition>()
                .As<IExecuteActivityDefinition<TActivity, TArguments>>();
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _builder.RegisterType<TDefinition>()
                .As<IEndpointDefinition<T>>();

            if (settings != null)
                _builder.RegisterInstance(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = context.Resolve<IClientFactory>();

                return context.TryResolve(out ConsumeContext consumeContext)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            });
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = context.Resolve<IClientFactory>();

                return context.TryResolve(out ConsumeContext consumeContext)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            });
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _builder.RegisterType<TActivity>();

            _builder.Register(CreateCompensateActivityScopeProvider<TActivity, TLog>)
                .As<ICompensateActivityScopeProvider<TActivity, TLog>>();
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IComponentContext context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, "message");
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IComponentContext context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, "message");
        }
    }
}
