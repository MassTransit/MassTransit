namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;


    public class ContainerBuilderMediatorConfigurator :
        RegistrationConfigurator,
        IContainerBuilderMediatorConfigurator
    {
        readonly AutofacContainerRegistrar _registrar;
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public ContainerBuilderMediatorConfigurator(ContainerBuilder builder)
            : this(builder, new AutofacContainerMediatorRegistrar(builder))
        {
        }

        ContainerBuilderMediatorConfigurator(ContainerBuilder builder, AutofacContainerRegistrar registrar)
            : base(registrar)
        {
            IMediatorRegistrationContext CreateRegistrationContext(IComponentContext context)
            {
                var registration = CreateRegistration(context.Resolve<IConfigurationServiceProvider>(), null);
                return new MediatorRegistrationContext(registration);
            }

            Builder = builder;
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

            builder.Register(CreateRegistrationContext)
                .As<IMediatorRegistrationContext>()
                .SingleInstance();
        }

        public string ScopeName
        {
            private get => _registrar.ScopeName;
            set => _registrar.ScopeName = value;
        }

        public ContainerBuilder Builder { get; }

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            get => _registrar.ConfigureScope;
            set => _registrar.ConfigureScope = value;
        }

        public void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        IMediator MediatorFactory(IComponentContext context)
        {
            var provider = context.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var registrationContext = context.Resolve<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(registrationContext, cfg);
                cfg.ConfigureConsumers(registrationContext);
                cfg.ConfigureSagas(registrationContext);
            });
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());
            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, ConfigureScope);
        }
    }
}
