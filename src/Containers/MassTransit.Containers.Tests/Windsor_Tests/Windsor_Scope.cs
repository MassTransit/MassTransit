namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Windsor_Scope :
        Common_Scope
    {
        readonly IWindsorContainer _container;

        public Windsor_Scope()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _container.Resolve<ISendEndpointProvider>();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _container.Resolve<IPublishEndpoint>();
        }
    }
}
