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
        Action<IContext, IReceiveEndpointConfigurator> _configure;

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
        }

        ConfigurationExpression IConfigurationExpressionMediatorConfigurator.Builder => _expression;

        public void ConfigureMediator(Action<IContext, IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        IMediator MediatorFactory(IContext context)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);

                ConfigureMediator(cfg, provider);
            });
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
        }
    }
}
