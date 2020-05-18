namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;


    [TestFixture]
    public class Lamar_ScopePublish :
        Common_ScopePublish<IContainer>
    {
        readonly IContainer _container;
        readonly INestedContainer _childContainer;

        public Lamar_ScopePublish()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
            _childContainer = _container.GetNestedContainer();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
            _container.Dispose();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _childContainer.GetInstance<IPublishEndpoint>();
        }

        protected override void AssertScopesAreEqual(IContainer actual)
        {
            Assert.AreEqual(_childContainer, actual);
        }
    }
}
