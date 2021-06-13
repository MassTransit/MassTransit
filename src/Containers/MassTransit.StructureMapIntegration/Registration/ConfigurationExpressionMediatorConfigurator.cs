namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;
    using StructureMap;


    public class ConfigurationExpressionMediatorConfigurator :
        RegistrationConfigurator,
        IConfigurationExpressionMediatorConfigurator
    {
        readonly ConfigurationExpression _expression;
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public ConfigurationExpressionMediatorConfigurator(ConfigurationExpression expression)
            : base(new StructureMapContainerMediatorRegistrar(expression))
        {
            _expression = expression;

            _expression.For<IMediator>()
                .Use(context => MediatorFactory(context))
                .Singleton();

            expression.For<IConsumerScopeProvider>()
                .Use(context => CreateConsumerScopeProvider(context))
                .Singleton();

            expression.For<IConfigurationServiceProvider>()
                .Use(context => new StructureMapConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            expression.For<IMediatorRegistrationContext>()
                .Use(context => CreateRegistrationContext(context))
                .Singleton();
        }

        ConfigurationExpression IConfigurationExpressionMediatorConfigurator.Builder => _expression;

        public void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        IMediatorRegistrationContext CreateRegistrationContext(IContext context)
        {
            var registration = CreateRegistration(context.GetInstance<IConfigurationServiceProvider>(), null);
            return new MediatorRegistrationContext(registration);
        }

        IMediator MediatorFactory(IContext context)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var registrationContext = context.GetInstance<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(registrationContext, cfg);
                cfg.ConfigureConsumers(registrationContext);
                cfg.ConfigureSagas(registrationContext);
            });
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
        }
    }
}
