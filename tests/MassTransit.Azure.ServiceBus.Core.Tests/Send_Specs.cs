namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_have_a_redelivery_flag_of_false()
        {
            ConsumeContext<PingMessage> context = await _handler;

            Assert.That(context.ReceiveContext.Redelivered, Is.False);
        }

        [Test]
        public async Task Should_succeed()
        {
            await _handler;
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }
    }


    [TestFixture]
    [Explicit]
    public class A_fault_on_the_receive_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_properly_reconnect_and_reconfigure_the_broker()
        {
            ConsumeContext<PingMessage> context = await _handler;
        }

        [Test]
        public async Task Should_succeed()
        {
            await _handler;
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Task.Delay(30000);

            await InputQueueSendEndpoint.Send(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }
}
