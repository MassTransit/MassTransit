namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;
    using StructureMap;


    public class ConfigurationExpressionRegistrationConfigurator :
        RegistrationConfigurator,
        IConfigurationExpressionConfigurator
    {
        readonly ConfigurationExpression _expression;

        public ConfigurationExpressionRegistrationConfigurator(ConfigurationExpression expression)
            : base(new StructureMapContainerRegistrar(expression))
        {
            _expression = expression;

            expression.For<IConsumerScopeProvider>()
                .Use(context => CreateConsumerScopeProvider(context))
                .Singleton();

            expression.For<ISagaRepositoryFactory>()
                .Use(context => CreateSagaRepositoryFactory(context))
                .Singleton();

            expression.For<IConfigurationServiceProvider>()
                .Use(context => new StructureMapConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            expression.For<IRegistrationConfigurator>()
                .Use(this);

            expression.For<IRegistration>()
                .Use(provider => CreateRegistration(provider.GetInstance<IConfigurationServiceProvider>()))
                .Singleton();
        }

        ConfigurationExpression IConfigurationExpressionConfigurator.Builder => _expression;

        public void AddBus(Func<IContext, IBusControl> busFactory)
        {
            _expression.For<IBusControl>()
                .Use(context => busFactory(context))
                .Singleton();

            _expression.For<IBus>()
                .Use(context => context.GetInstance<IBusControl>())
                .Singleton();

            _expression.For<ISendEndpointProvider>()
                .Use(context => GetCurrentSendEndpointProvider(context))
                .ContainerScoped();

            _expression.For<IPublishEndpoint>()
                .Use(context => GetCurrentPublishEndpoint(context))
                .ContainerScoped();

            _expression.For<IClientFactory>()
                .Use(context => context.GetInstance<IBus>().CreateClientFactory(default))
                .Singleton();
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
        }

        ISagaRepositoryFactory CreateSagaRepositoryFactory(IContext context)
        {
            return new StructureMapSagaRepositoryFactory(context.GetInstance<IContainer>());
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IContext context)
        {
            return context.TryGetInstance<ConsumeContext>() ?? (ISendEndpointProvider)context.GetInstance<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IContext context)
        {
            return context.TryGetInstance<ConsumeContext>() ?? (IPublishEndpoint)context.GetInstance<IBus>();
        }
    }
}
