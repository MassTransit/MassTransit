namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_Scope :
        Common_Scope
    {
        readonly Container _container;

        public SimpleInjector_Scope()
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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseLog(Console.Out, log =>
                Task.FromResult(
                    $"Received (input_queue): {log.Context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join((string)",", (IEnumerable<string>)log.Context.SupportedMessageTypes)})"));

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
