namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;
    using StructureMap;


    [TestFixture]
    public class StructureMap_ScopePublish :
        Common_ScopePublish<IContainer>
    {
        readonly IContainer _container;

        public StructureMap_ScopePublish()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IPublishScopeProvider GetPublishScopeProvider()
        {
            return _container.GetInstance<IPublishScopeProvider>();
        }
    }
}
