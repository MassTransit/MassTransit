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
            _builder.Register(busFactory)
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();

            _builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            _builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();

            _builder.Register(context => context.Resolve<IBus>().CreateClientFactory())
                .As<IClientFactory>()
                .SingleInstance();
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
