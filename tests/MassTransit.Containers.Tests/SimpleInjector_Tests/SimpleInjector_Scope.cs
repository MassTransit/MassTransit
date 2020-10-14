namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using SimpleInjector;


    [TestFixture]
    public class SimpleInjector_Scope :
        Common_Scope
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjector_Scope()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddBus(() => BusControl);
            });
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseExecute(context => Console.WriteLine(
                $"Received (input_queue): {context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", context.SupportedMessageTypes)})"));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _container.GetInstance<ISendEndpointProvider>();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _container.GetInstance<IPublishEndpoint>();
        }
    }
}
