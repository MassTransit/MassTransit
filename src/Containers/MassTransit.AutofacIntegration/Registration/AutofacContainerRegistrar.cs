namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using Automatonymous;
    using Clients;
    using Courier;
    using Definition;
    using Futures;
    using MassTransit.Registration;
    using Mediator;
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

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope { get; set; }
        public string ScopeName { get; set; }

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

        public void RegisterSagaStateMachine<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _builder.RegisterType<TStateMachine>()
                .AsSelf()
                .As<SagaStateMachine<TInstance>>()
                .SingleInstance();
        }

        public void RegisterSagaRepository<TSaga>(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
            where TSaga : class, ISaga
        {
            RegisterSingleInstance(provider => repositoryFactory(provider));
        }

        void IContainerRegistrar.RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _builder.RegisterType<TConsumeContextFactory>().As<ISagaConsumeContextFactory<TContext, TSaga>>();
            _builder.RegisterType<TRepositoryContextFactory>().As<ISagaRepositoryContextFactory<TSaga>>();

            _builder.Register(context =>
            {
                var lifetimeScopeProvider = context.ResolveOptional<ILifetimeScopeProvider>()
                    ?? new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

                return new AutofacSagaRepositoryContextFactory<TSaga>(lifetimeScopeProvider, ScopeName, ConfigureScope);
            });

            _builder.Register<ISagaRepository<TSaga>>(context => new SagaRepository<TSaga>(context.Resolve<AutofacSagaRepositoryContextFactory<TSaga>>()))
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

        public void RegisterFuture<TFuture>()
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _builder.RegisterType<TFuture>()
                .AsSelf()
                .SingleInstance();
        }

        public void RegisterFutureDefinition<TDefinition, TFuture>()
            where TDefinition : class, IFutureDefinition<TFuture>
            where TFuture : MassTransitStateMachine<FutureState>
        {
            _builder.RegisterType<TDefinition>()
                .As<IFutureDefinition<TFuture>>();
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = GetClientFactory(context);

                if (context.TryResolve(out ConsumeContext consumeContext))
                    return clientFactory.CreateRequestClient<T>(consumeContext, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<ILifetimeScope>(clientFactory, context.Resolve<ILifetimeScope>()))
                    .CreateRequestClient<T>(timeout);
            }).InstancePerLifetimeScope();
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = GetClientFactory(context);

                if (context.TryResolve(out ConsumeContext consumeContext))
                    return clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);

                return new ClientFactory(new ScopedClientFactoryContext<ILifetimeScope>(clientFactory, context.Resolve<ILifetimeScope>()))
                    .CreateRequestClient<T>(destinationAddress, timeout);
            }).InstancePerLifetimeScope();
        }

        public void Register<T, TImplementation>()
            where T : class
            where TImplementation : class, T
        {
            _builder.RegisterType<TImplementation>().As<T>().InstancePerLifetimeScope();
        }

        public void Register<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _builder.Register(context => factoryMethod(new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))).InstancePerLifetimeScope();
        }

        public void RegisterSingleInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class
        {
            _builder.Register(context => factoryMethod(new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))).SingleInstance();
        }

        public void RegisterSingleInstance<T>(T instance)
            where T : class
        {
            _builder.RegisterInstance(instance);
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

            return new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IComponentContext context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        protected virtual IClientFactory GetClientFactory(IComponentContext componentContext)
        {
            return componentContext.Resolve<IClientFactory>();
        }
    }


    public class AutofacContainerRegistrar<TBus> :
        AutofacContainerRegistrar
    {
        public AutofacContainerRegistrar(ContainerBuilder builder)
            : base(builder)
        {
        }

        protected override IClientFactory GetClientFactory(IComponentContext componentContext)
        {
            return componentContext.Resolve<Bind<TBus, IClientFactory>>().Value;
        }
    }


    public class AutofacContainerMediatorRegistrar :
        AutofacContainerRegistrar
    {
        public AutofacContainerMediatorRegistrar(ContainerBuilder builder)
            : base(builder)
        {
        }

        protected override IClientFactory GetClientFactory(IComponentContext componentContext)
        {
            return componentContext.Resolve<IMediator>();
        }
    }
}
