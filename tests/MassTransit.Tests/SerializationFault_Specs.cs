namespace MassTransit.Tests
{
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_a_message_fails_to_deserialize_properly :
        InMemoryTestFixture
    {
        [Test]
        public void It_should_respond_with_a_serialization_fault()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestFaultException>());
        }

        IRequestClient<PingMessage> _requestClient;
        Task<Response<PongMessage>> _response;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context => throw new SerializationException("This is fine, forcing death"));
        }
    }


    /// <summary>
    /// this requires debugger tricks to make it work
    /// </summary>
    [TestFixture]
    [Explicit]
    public class When_a_message_has_an_unrecognized_body_format :
        InMemoryTestFixture
    {
        [Test]
        public async Task It_should_publish_a_fault()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), context => context.ContentType = new ContentType("text/json"));

            ConsumeContext<ReceiveFault> faultContext = await _faulted;
        }

        Task<ConsumeContext<PingMessage>> _handled;
        Task<ConsumeContext<ReceiveFault>> _faulted;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);

            _faulted = Handled<ReceiveFault>(configurator);
        }
    }
}
