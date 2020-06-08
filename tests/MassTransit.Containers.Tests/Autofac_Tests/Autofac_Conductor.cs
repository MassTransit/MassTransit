namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using Monitoring.Health;
    using NUnit.Framework;
    using Registration;


    public class Autofac_Conductor :
        Common_Conductor
    {
        readonly IContainer _container;

        public Autofac_Conductor(bool instanceEndpoint)
            : base(instanceEndpoint)
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
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
