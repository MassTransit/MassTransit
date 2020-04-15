namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Lamar_ScopePublish :
        Common_ScopePublish<INestedContainer>
    {
        readonly IContainer _container;

        public Lamar_ScopePublish()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IPublishScopeProvider GetPublishScopeProvider()
        {
            return _container.GetInstance<IPublishScopeProvider>();
        }
    }
}
