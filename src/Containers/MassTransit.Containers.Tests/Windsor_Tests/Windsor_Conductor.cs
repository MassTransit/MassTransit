namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;


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
