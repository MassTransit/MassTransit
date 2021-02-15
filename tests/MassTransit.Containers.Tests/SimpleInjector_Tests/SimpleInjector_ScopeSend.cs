namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_ScopeSend :
        Common_ScopeSend<Container>
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;
        readonly Scope _childContainer;

        public SimpleInjector_ScopeSend()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg =>
            {
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

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _childContainer.GetInstance<ISendEndpointProvider>();
        }

        protected override void AssertScopesAreEqual(Container actual)
        {
            Assert.AreEqual(_childContainer.Container, actual);
        }
    }
}
