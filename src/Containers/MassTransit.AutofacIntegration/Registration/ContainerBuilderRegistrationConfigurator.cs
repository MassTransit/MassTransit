namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using Autofac.Core;
    using MassTransit.Registration;
    using Mediator;
    using Monitoring.Health;
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

            builder.RegisterType<BusRegistry>().As<IBusRegistry>().SingleInstance();

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

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
        }

        public void AddBus(Func<IRegistrationContext<IComponentContext>, IBusControl> busFactory)
        {
            ThrowIfAlreadyConfigured();

            IBusControl BusFactory(IComponentContext componentContext)
            {
                var provider = componentContext.Resolve<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                IRegistrationContext<IComponentContext> context = GetRegistrationContext(componentContext);

                return busFactory(context);
            }

            if (_builder.ComponentRegistryBuilder.IsRegistered(new TypedService(typeof(IBusControl))))
            {
                throw new ConfigurationException(
                    "AddBus() was already called. To configure multiple bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
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

            _builder.Register(context => ClientFactoryProvider(context.Resolve<IConfigurationServiceProvider>(), context.Resolve<IBus>()))
                .As<IClientFactory>()
                .SingleInstance();

            _builder.Register(context => new BusHealth(nameof(IBus)))
                .As<BusHealth>()
                .As<IBusHealth>()
                .SingleInstance();

            _builder.RegisterType<BusRegistryInstance>()
                .As<IBusRegistryInstance>()
                .SingleInstance();
        }

        public void AddMediator(Action<IComponentContext, IReceiveEndpointConfigurator> configure = null)
        {
            ThrowIfAlreadyConfigured();

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

        IRegistrationContext<IComponentContext> GetRegistrationContext(IComponentContext context)
        {
            return new RegistrationContext<IComponentContext>(
                CreateRegistration(context.Resolve<IConfigurationServiceProvider>()),
                context.Resolve<BusHealth>(),
                context
            );
        }
    }
}
