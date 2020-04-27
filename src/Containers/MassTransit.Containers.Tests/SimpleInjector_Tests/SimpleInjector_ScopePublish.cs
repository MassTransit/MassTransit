namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_ScopePublish :
        Common_ScopePublish<Container>
    {
        readonly Container _container;
        readonly Scope _childContainer;

        public SimpleInjector_ScopePublish()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddBus(() => BusControl);
            });
            _childContainer = AsyncScopedLifestyle.BeginScope(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
            _container.Dispose();
        }

        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseExecute(context => Console.WriteLine(
                $"Received (input_queue): {context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", context.SupportedMessageTypes)})"));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _childContainer.GetInstance<IPublishEndpoint>();
        }

        protected override void AssertScopesAreEqual(Container actual)
        {
            Assert.AreEqual(_childContainer.Container, actual);
        }
    }
}
