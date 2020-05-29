namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class Sending_with_virtual_host : AmazonSqsTestFixture
    {
        Task<ConsumeContext<Ping>> _handled;

        protected override void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            base.ConfigureAmazonSqsHost(configurator);
            configurator.Scope("test");
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            base.ConfigureAmazonSqsReceiveEndpoint(configurator);
            _handled = Handled<Ping>(configurator);
        }

        [Test]
        public async Task Should_be_able_to_handle()
        {
            await InputQueueSendEndpoint.Send<Ping>(new { });
            await _handled;
        }


        public interface Ping
        {
        }
    }


    public class Publishing_with_virtual_host : AmazonSqsTestFixture
    {
        Task<ConsumeContext<Ping>> _handled;

        protected override void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            base.ConfigureAmazonSqsHost(configurator);
            configurator.Scope("test");
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            base.ConfigureAmazonSqsReceiveEndpoint(configurator);
            _handled = Handled<Ping>(configurator);
        }

        [Test]
        public async Task Should_be_able_to_handle()
        {
            await Bus.Publish<Ping>(new { });

            ConsumeContext<Ping> context = await _handled;

            var entityName = new AmazonSqsMessageNameFormatter().GetMessageName(typeof(Ping));

            Assert.That(context.DestinationAddress, Is.EqualTo(new Uri($"amazonsqs://localhost/test/{entityName}")));
        }


        public interface Ping
        {
        }
    }
}
