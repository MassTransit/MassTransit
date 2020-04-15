namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;


    public class ContainerBuilderRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderConfigurator
    {
        readonly ContainerBuilder _builder;
        Action<ContainerBuilder, ConsumeContext> _configureScope;

        public ContainerBuilderRegistrationConfigurator(ContainerBuilder builder)
            : base(new AutofacContainerRegistrar(builder))
        {
            _builder = builder;

            ScopeName = "message";

            builder.Register(CreateSendScopeProvider)
                .As<ISendScopeProvider>()
                .SingleInstance();

            builder.Register(CreatePublishScopeProvider)
                .As<IPublishScopeProvider>()
                .SingleInstance();

            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance();

            builder.Register(CreateSagaRepositoryFactory)
                .As<ISagaRepositoryFactory>()
                .SingleInstance();

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance();

            builder.RegisterInstance<IRegistrationConfigurator>(this);

            builder.Register(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                .As<IRegistration>()
                .SingleInstance();
        }

        public string ScopeName { private get; set; }

        ContainerBuilder IContainerBuilderConfigurator.Builder => _builder;

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            set => _configureScope = value;
        }

        public void AddBus(Func<IComponentContext, IBusControl> busFactory)
        {
            IBusControl BusFactory(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(context);
            }

            _builder.Register(BusFactory)
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();

            _builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            _builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();

            _builder.Register(context => ClientFactoryProvider(context.Resolve<IConfigurationServiceProvider>()))
                .As<IClientFactory>()
                .SingleInstance();
        }

        public void AddMediator(Action<IComponentContext, IReceiveEndpointConfigurator> configure = null)
        {
            IMediator MediatorFactory(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(context, cfg);

                    ConfigureMediator(cfg, provider);
                });
            }

            _builder.Register(MediatorFactory)
                .As<IMediator>()
                .As<IClientFactory>()
                .SingleInstance();
        }

        ISendScopeProvider CreateSendScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacSendScopeProvider(lifetimeScopeProvider, ScopeName);
        }

        IPublishScopeProvider CreatePublishScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacPublishScopeProvider(lifetimeScopeProvider, ScopeName);
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, _configureScope);
        }

        ISagaRepositoryFactory CreateSagaRepositoryFactory(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacSagaRepositoryFactory(lifetimeScopeProvider, ScopeName, _configureScope);
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return context.Resolve<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return context.Resolve<IBus>();
        }
    }
}
