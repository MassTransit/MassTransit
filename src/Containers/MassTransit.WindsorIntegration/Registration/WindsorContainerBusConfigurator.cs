namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using Transactions;
    using Transports;


    public class WindsorContainerBusConfigurator :
        RegistrationConfigurator,
        IWindsorContainerBusConfigurator
    {
        public WindsorContainerBusConfigurator(IWindsorContainer container)
            : base(new WindsorContainerRegistrar(container))
        {
            IBusRegistrationContext CreateRegistrationContext(IKernel context)
            {
                var provider = context.Resolve<IConfigurationServiceProvider>();
                return new BusRegistrationContext(provider, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            Container = container;

            if (!container.Kernel.HasComponent(typeof(IBusDepot)))
                container.Register(Component.For<IBusDepot>().ImplementedBy<BusDepot>().LifestyleSingleton());

            container.RegisterScopedContextProviderIfNotPresent();

            if (!container.Kernel.HasComponent(typeof(IConsumerScopeProvider)))
            {
                container.Register(Component.For<IConsumerScopeProvider>()
                    .ImplementedBy<WindsorConsumerScopeProvider>()
                    .LifestyleTransient());
            }

            if (!container.Kernel.HasComponent(typeof(IConfigurationServiceProvider)))
            {
                container.Register(Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton());
            }

            container.Register(
                Component.For<IBusRegistrationContext>()
                    .UsingFactoryMethod(provider => CreateRegistrationContext(provider))
                    .LifestyleSingleton(),
                Component.For<ISendEndpointProvider>()
                    .UsingFactoryMethod(GetCurrentSendEndpointProvider)
                    .LifestyleTransient(),
                Component.For<IPublishEndpoint>()
                    .UsingFactoryMethod(GetCurrentPublishEndpoint)
                    .LifestyleTransient(),
                Component.For<IClientFactory>()
                    .UsingFactoryMethod(kernel => ClientFactoryProvider(kernel.Resolve<IConfigurationServiceProvider>(), kernel.Resolve<IBus>()))
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container { get; }

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

            Container.Register(
                Component.For<IBusInstance>()
                    .Forward<IReceiveEndpointConnector>()
                    .UsingFactoryMethod(kernel => busFactory.CreateBus(kernel.Resolve<IBusRegistrationContext>()))
                    .LifestyleSingleton(),
                Component.For<IBusControl>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IBusInstance>().BusControl)
                    .LifestyleSingleton(),
                Component.For<IBus>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IBusInstance>().Bus)
                    .LifestyleSingleton(),
            #pragma warning disable 618
                Component.For<IBusHealth>()
                    .UsingFactoryMethod(kernel => new BusHealth(kernel.Resolve<IBusInstance>()))
                    .LifestyleSingleton()
            );

            Registrar.RegisterScopedClientFactory();
        }

        public void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            throw new NotSupportedException("Riders are only supported with Microsoft DI and Autofac");
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IKernel context)
        {
            return (ISendEndpointProvider)context.GetConsumeContext()
                ?? new ScopedSendEndpointProvider<IKernel>(context.TryResolve<ITransactionalBus>() ?? context.Resolve<IBus>(), context);
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IKernel context)
        {
            return (IPublishEndpoint)context.GetConsumeContext()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<IKernel>(context.TryResolve<ITransactionalBus>() ?? context.Resolve<IBus>(), context));
        }
    }
}
