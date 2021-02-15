namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework.Messages;


    [TestFixture]
    public class SimpleInjector_ScopeRequestClient :
        Common_ScopeRequestClient<Container>
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;
        readonly Scope _childContainer;

        public SimpleInjector_ScopeRequestClient()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                cfg.AddRequestClient<PingMessage>();
                cfg.AddBus(() => BusControl);
            });
            _childContainer = AsyncScopedLifestyle.BeginScope(_container);
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _childContainer.DisposeAsync();
            await _container.DisposeAsync();
        }

        protected override IRequestClient<SimpleMessageClass> GetSendRequestClient()
        {
            return _childContainer.GetInstance<IRequestClient<SimpleMessageClass>>();
        }

        protected override IRequestClient<PingMessage> GetPublishRequestClient()
        {
            return _childContainer.GetInstance<IRequestClient<PingMessage>>();
        }

        protected override void AssertScopesAreEqual(Container actual)
        {
            Assert.AreEqual(_childContainer.Container, actual);
        }
    }
}
