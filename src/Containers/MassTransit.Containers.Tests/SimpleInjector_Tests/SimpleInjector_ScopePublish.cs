namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using Scoping;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_ScopePublish :
        Common_ScopePublish<Scope>
    {
        readonly Container _container;

        public SimpleInjector_ScopePublish()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddBus(() => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
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

        protected override IPublishScopeProvider GetPublishScopeProvider()
        {
            return _container.GetInstance<IPublishScopeProvider>();
        }
    }
}
