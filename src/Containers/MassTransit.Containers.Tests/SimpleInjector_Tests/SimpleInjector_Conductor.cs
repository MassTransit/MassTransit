namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjector_Conductor :
        Common_Conductor
    {
        readonly Container _container;

        public SimpleInjector_Conductor(bool instanceEndpoint)
            : base(instanceEndpoint)
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureServiceEndpoints(IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(_container.GetInstance<Container>(), Options);
        }

        protected override IClientFactory GetClientFactory()
        {
            return _container.GetInstance<IClientFactory>();
        }
    }
}
