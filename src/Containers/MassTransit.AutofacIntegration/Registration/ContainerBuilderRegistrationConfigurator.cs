namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using GreenPipes;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ContainerBuilderRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderConfigurator
    {
        readonly ContainerBuilder _builder;
        readonly AutofacContainerRegistrar _registrar;

        public ContainerBuilderRegistrationConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerRegistrar(builder))
        {
        }

        ContainerBuilderRegistrationConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            _builder = builder;
            _registrar = registrar;

            ScopeName = "message";

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

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        ContainerBuilder IContainerBuilderConfigurator.Builder => _builder;

        public Action<ContainerBuilder, SendContext> ConfigureSendScope { get; set; }
        public Action<ContainerBuilder, PublishContext> ConfigurePublishScope { get; set; }

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
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

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        ISagaRepositoryFactory CreateSagaRepositoryFactory(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacSagaRepositoryFactory(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return new ScopedSendEndpointProvider<ILifetimeScope>(context.Resolve<IBus>(), context.Resolve<ILifetimeScope>());
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return new PublishEndpoint(new ScopedPublishEndpointProvider<ILifetimeScope>(context.Resolve<IBus>(), context.Resolve<ILifetimeScope>()));
        }
    }
}
