namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using Monitoring.Health;
    using NUnit.Framework;
    using Registration;


    public class Lamar_Conductor :
        Common_Conductor
    {
        readonly IContainer _container;

        public Lamar_Conductor(bool instanceEndpoint)
            : base(instanceEndpoint)
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureServiceEndpoints(IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(GetRegistrationContext(), Options);
        }

        IRegistrationContext GetRegistrationContext()
        {
            return new RegistrationContext(_container.GetInstance<IRegistration>(), _container.GetInstance<BusHealth>());
        }

        protected override IClientFactory GetClientFactory()
        {
            return _container.GetInstance<IClientFactory>();
        }
    }
}
