namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;


    public class SimpleInjectorRegistrationConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorConfigurator
    {
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorRegistrationConfigurator(Container container)
            : base(new SimpleInjectorContainerRegistar(container))
        {
            Container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);

            AddMassTransitComponents(Container);

            Container.RegisterInstance<IRegistrationConfigurator>(this);

            Container.RegisterSingleton(() => CreateRegistration(container.GetInstance<IConfigurationServiceProvider>()));
        }

        public Container Container { get; }

        public void AddBus(Func<IBusControl> busFactory)
        {
            Container.RegisterSingleton(busFactory);

            Container.RegisterSingleton<IBus>(() => Container.GetInstance<IBusControl>());

            Container.Register(GetSendEndpointProvider, _hybridLifestyle);

            Container.Register(GetPublishEndpoint, _hybridLifestyle);

            Container.RegisterSingleton(() => Container.GetInstance<IBus>().CreateClientFactory());
        }

        ISendEndpointProvider GetSendEndpointProvider()
        {
            return (ISendEndpointProvider)Container.GetConsumeContext() ?? Container.GetInstance<IBus>();
        }

        IPublishEndpoint GetPublishEndpoint()
        {
            return (IPublishEndpoint)Container.GetConsumeContext() ?? Container.GetInstance<IBus>();
        }

        static void AddMassTransitComponents(Container container)
        {
            container.Register<ScopedConsumeContextProvider>(Lifestyle.Scoped);

            container.Register(() => container.GetInstance<ScopedConsumeContextProvider>().GetContext(), Lifestyle.Scoped);

            container.RegisterSingleton<IConsumerScopeProvider>(() => new SimpleInjectorConsumerScopeProvider(container));
            container.RegisterSingleton<ISagaRepositoryFactory>(() => new SimpleInjectorSagaRepositoryFactory(container));
            container.RegisterSingleton<IConfigurationServiceProvider>(() => new SimpleInjectorConfigurationServiceProvider(container));
        }
    }
}
