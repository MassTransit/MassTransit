namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework.Messages;


    [TestFixture]
    public class SimpleInjector_ScopeRequestClient :
        Common_ScopeRequestClient<Container>
    {
        readonly Container _container;
        readonly Scope _childContainer;

        public SimpleInjector_ScopeRequestClient()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                cfg.AddRequestClient<PingMessage>();
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
