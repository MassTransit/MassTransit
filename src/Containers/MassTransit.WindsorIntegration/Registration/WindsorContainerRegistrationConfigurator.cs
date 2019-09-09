namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;


    public class WindsorContainerRegistrationConfigurator :
        RegistrationConfigurator,
        IWindsorContainerConfigurator
    {
        readonly IWindsorContainer _container;

        public WindsorContainerRegistrationConfigurator(IWindsorContainer container)
            : base(new WindsorContainerRegistrar(container))
        {
            _container = container;

            container.Register(
                Component.For<ScopedConsumeContextProvider>().LifestyleScoped(),
                Component.For<ConsumeContext>().UsingFactoryMethod(kernel => kernel.Resolve<ScopedConsumeContextProvider>().GetContext()).LifestyleScoped(),
                Component.For<IConsumerScopeProvider>().ImplementedBy<WindsorConsumerScopeProvider>().LifestyleTransient(),
                Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton(),
                Component.For<ISagaRepositoryFactory>()
                    .ImplementedBy<WindsorSagaRepositoryFactory>()
                    .LifestyleSingleton(),
                Component.For<IRegistrationConfigurator>()
                    .Instance(this)
                    .LifestyleSingleton(),
                Component.For<MassTransit.IRegistration>()
                    .UsingFactoryMethod(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container => _container;

        public void AddBus(Func<IKernel, IBusControl> busFactory)
        {
            _container.Register(
                Component.For<IBusControl>()
                    .Forward<IBus>()
                    .UsingFactoryMethod(busFactory).LifestyleSingleton(),
                Component.For<ISendEndpointProvider>()
                    .UsingFactoryMethod(GetCurrentSendEndpointProvider)
                    .LifestyleTransient(),
                Component.For<IPublishEndpoint>()
                    .UsingFactoryMethod(GetCurrentPublishEndpoint)
                    .LifestyleTransient(),
                Component.For<IClientFactory>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IBus>().CreateClientFactory())
                    .LifestyleSingleton()
            );
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IKernel kernel)
        {
            return new WindsorConsumerScopeProvider(kernel);
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IKernel context)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            return currentScope != null
                ? context.Resolve<ConsumeContext>()
                : (ISendEndpointProvider)context.Resolve<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IKernel context)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            return currentScope != null
                ? context.Resolve<ConsumeContext>()
                : (IPublishEndpoint)context.Resolve<IBus>();
        }
    }
}
