namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ContainerBuilderBusRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderBusConfigurator
    {
        readonly ContainerBuilder _builder;
        readonly AutofacContainerRegistrar _registrar;

        public ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerRegistrar(builder))
        {
        }

        ContainerBuilderBusRegistrationConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            IBusRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                var busHealth = context.Resolve<BusHealth>();
                return new BusRegistrationContext(provider, busHealth, EndpointRegistrations, ConsumerRegistrations, SagaRegistrations,
                    ExecuteActivityRegistrations, ActivityRegistrations);
            }

            _builder = builder;
            _registrar = registrar;

            ScopeName = "message";

            builder.RegisterType<BusDepot>()
                .As<IBusDepot>()
                .SingleInstance();

            _builder.Register(context => new BusHealth(nameof(IBus)))
                .As<BusHealth>()
                .As<IBusHealth>()
                .SingleInstance();

            _builder.RegisterType<DefaultBusInstance>()
                .As<IBusInstance>()
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

            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConsumerScopeProvider));

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConfigurationServiceProvider));

            builder.Register(provider => CreateRegistrationContext(provider))
                .As<IBusRegistrationContext>()
                .SingleInstance();
        }

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        ContainerBuilder IContainerBuilderBusConfigurator.Builder => _builder;

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
        }

        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(AddBus));

            _builder.Register(context => BusFactory(context, busFactory))
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            throw new NotImplementedException();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotImplementedException();
        }

        IBusControl BusFactory(IComponentContext componentContext, Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            var provider = componentContext.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = componentContext.Resolve<IBusRegistrationContext>();

            return busFactory(context);
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, ConfigureScope);
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
