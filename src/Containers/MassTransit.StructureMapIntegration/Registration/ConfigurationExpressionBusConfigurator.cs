namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using StructureMap;
    using Transactions;
    using Transports;


    public class ConfigurationExpressionBusConfigurator :
        RegistrationConfigurator,
        IConfigurationExpressionBusConfigurator
    {
        readonly ConfigurationExpression _expression;

        public ConfigurationExpressionBusConfigurator(ConfigurationExpression expression)
            : base(new StructureMapContainerRegistrar(expression))
        {
            _expression = expression;

            expression.For<IBusDepot>()
                .Use<BusDepot>()
                .Singleton();

            _expression.For<ISendEndpointProvider>()
                .Use(context => GetCurrentSendEndpointProvider(context))
                .ContainerScoped();

            _expression.For<IPublishEndpoint>()
                .Use(context => GetCurrentPublishEndpoint(context))
                .ContainerScoped();

            _expression.For<IClientFactory>()
                .Use(context => ClientFactoryProvider(context.GetInstance<IConfigurationServiceProvider>(), context.GetInstance<IBus>()))
                .Singleton();

            _expression.For<BusHealth>()
                .Use(context => new BusHealth())
                .Singleton();

            _expression.For<IBusHealth>()
                .Use<BusHealth>()
                .Singleton();

            expression.For<IConsumerScopeProvider>()
                .Use(context => CreateConsumerScopeProvider(context))
                .Singleton();

            expression.For<IConfigurationServiceProvider>()
                .Use(context => new StructureMapConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            expression.For<IBusRegistrationContext>()
                .Use(context => CreateRegistrationContext(context))
                .Singleton();
        }

        ConfigurationExpression IConfigurationExpressionBusConfigurator.Builder => _expression;

        public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            _expression.For<IBusInstance>()
                .Use(context => busFactory.CreateBus(context.GetInstance<IBusRegistrationContext>(), null))
                .Singleton();

            _expression.Forward<IBusInstance, IReceiveEndpointConnector>();

            _expression.For<IBusControl>()
                .Use(context => context.GetInstance<IBusInstance>().BusControl)
                .Singleton();

            _expression.For<IBus>()
                .Use(context => context.GetInstance<IBusInstance>().Bus)
                .Singleton();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotSupportedException("Riders are only supported with Microsoft DI and Autofac");
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IContext context)
        {
            return (ISendEndpointProvider)context.TryGetInstance<ConsumeContext>()
                ?? new ScopedSendEndpointProvider<IContainer>(context.TryGetInstance<ITransactionalBus>() ?? context.GetInstance<IBus>(),
                    context.GetInstance<IContainer>());
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IContext context)
        {
            return (IPublishEndpoint)context.TryGetInstance<ConsumeContext>()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<IContainer>(context.TryGetInstance<ITransactionalBus>() ?? context.GetInstance<IBus>(),
                    context.GetInstance<IContainer>()));
        }

        IBusRegistrationContext CreateRegistrationContext(IContext context)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();
            var busHealth = context.GetInstance<BusHealth>();
            return new BusRegistrationContext(provider, busHealth, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
        }
    }
}
