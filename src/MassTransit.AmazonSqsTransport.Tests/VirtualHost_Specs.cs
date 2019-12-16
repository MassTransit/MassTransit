namespace MassTransit.AmazonSqsTransport.Tests
{
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;


    public class Sending_with_virtual_host : AmazonSqsTestFixture
    {
        Task<ConsumeContext<IPing>> _handled;

        interface IPing
        {
        }


        protected override void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            base.ConfigureAmazonSqsHost(configurator);
            configurator.VirtualHost("test");
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            base.ConfigureAmazonSqsReceiveEndpoint(configurator);
            _handled = Handled<IPing>(configurator);
        }

        [Test]
        public async Task Should_be_able_to_respond()
        {
            await InputQueueSendEndpoint.Send<IPing>(new { });
            await _handled;
        }
    }

    public class Publishing_with_virtual_host : AmazonSqsTestFixture
    {
        Task<ConsumeContext<IPing>> _handled;

        interface IPing
        {
        }


        protected override void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            base.ConfigureAmazonSqsHost(configurator);
            configurator.VirtualHost("test");
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            base.ConfigureAmazonSqsReceiveEndpoint(configurator);
            _handled = Handled<IPing>(configurator);
        }

        [Test]
        public async Task Should_be_able_to_respond()
        {
            await Bus.Publish<IPing>(new { });
            await _handled;
        }
    }
}
