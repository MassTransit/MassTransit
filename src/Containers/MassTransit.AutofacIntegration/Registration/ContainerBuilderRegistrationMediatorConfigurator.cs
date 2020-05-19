namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;


    public class ContainerBuilderRegistrationMediatorConfigurator :
        RegistrationConfigurator,
        IContainerBuilderMediatorConfigurator
    {
        readonly ContainerBuilder _builder;
        readonly AutofacContainerRegistrar _registrar;
        Action<IComponentContext, IReceiveEndpointConfigurator> _configure;

        public ContainerBuilderRegistrationMediatorConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerMediatorRegistrar(builder))
        {
        }

        ContainerBuilderRegistrationMediatorConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            _builder = builder;
            _registrar = registrar;

            ScopeName = "message";

            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConsumerScopeProvider));

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConfigurationServiceProvider));

            builder.Register(MediatorFactory)
                .As<IMediator>()
                .SingleInstance();
        }

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        ContainerBuilder IContainerBuilderMediatorConfigurator.Builder => _builder;

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
        }

        IMediator MediatorFactory(IComponentContext context)
        {
            var provider = context.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);

                ConfigureMediator(cfg, provider);
            });
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());
            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }

        public void ConfigureMediator(Action<IComponentContext, IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured();
            _configure = configure;
        }
    }
}
