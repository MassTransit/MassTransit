namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using Monitoring.Health;
    using NUnit.Framework;
    using Registration;


    public class Windsor_Conductor :
        Common_Conductor
    {
        readonly IWindsorContainer _container;

        public Windsor_Conductor(bool instanceEndpoint)
            : base(instanceEndpoint)
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);
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
            return new RegistrationContext(_container.Resolve<IRegistration>(), _container.Resolve<BusHealth>());
        }

        protected override IClientFactory GetClientFactory()
        {
            return _container.Resolve<IClientFactory>();
        }
    }
}
