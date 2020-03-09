namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;


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
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureServiceEndpoints(IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator> configurator)
        {
            configurator.ConfigureServiceEndpoints(_container, Options);
        }

        protected override IClientFactory GetClientFactory()
        {
            return _container.Resolve<IClientFactory>();
        }
    }
}
