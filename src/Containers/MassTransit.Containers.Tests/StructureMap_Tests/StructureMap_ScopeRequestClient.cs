namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using StructureMap;
    using TestFramework.Messages;


    [TestFixture]
    public class StructureMap_ScopeRequestClient :
        Common_ScopeRequestClient<IContainer>
    {
        readonly IContainer _container;
        readonly IContainer _childContainer;

        public StructureMap_ScopeRequestClient()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                    cfg.AddRequestClient<PingMessage>();
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
        protected override IRequestClient<SimpleMessageClass> GetSendRequestClient()
        {
            return _childContainer.GetInstance<IRequestClient<SimpleMessageClass>>();
        }

        protected override IRequestClient<PingMessage> GetPublishRequestClient()
        {
            return _childContainer.GetInstance<IRequestClient<PingMessage>>();
        }

        protected override void AssertScopesAreEqual(IContainer actual)
        {
            Assert.AreEqual(_childContainer, actual);
        }
    }
}
