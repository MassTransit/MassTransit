namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;


    [TestFixture]
    public class StructureMap_ScopePublish :
        Common_ScopePublish<IContainer>
    {
        readonly IContainer _container;
        readonly IContainer _childContainer;

        public StructureMap_ScopePublish()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
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
