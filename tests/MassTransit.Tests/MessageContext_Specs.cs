namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_an_empty_fault_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.FaultAddress, Is.Null);
        }

        [Test]
        public async Task Should_have_an_empty_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.ResponseAddress, Is.Null);
        }

        [Test]
        public async Task Should_include_the_correlation_id()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.CorrelationId, Is.EqualTo(_correlationId));
        }

        [Test]
        public async Task Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.DestinationAddress, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public async Task Should_include_the_header()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.Headers.TryGetHeader("One", out var header);
            Assert.That(header, Is.EqualTo("1"));
        }

        [Test]
        public async Task Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.SourceAddress, Is.EqualTo(BusAddress));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _ping;
        #pragma warning restore NUnit1032
        Guid _correlationId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.New<SendContext<PingMessage>>(x => x.UseExecute(context =>
            {
                context.CorrelationId = _correlationId;
                context.Headers.Set("One", "1");
            })));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_response_on_the_handler()
        {
            Response<PongMessage> message = await _request;

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        [Test]
        public async Task Should_have_the_matching_correlation_id()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;

            Assert.That(context.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        [Test]
        public async Task Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.DestinationAddress, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public async Task Should_include_the_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.ResponseAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            Assert.That(ping.SourceAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;

            Assert.That(context.ConversationId, Is.EqualTo(_conversationId));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Response<PongMessage>> _request;
        #pragma warning restore NUnit1032
        Guid? _conversationId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();
            _conversationId = NewId.NextGuid();

            _request = Bus.Request<PingMessage, PongMessage>(InputQueueAddress, new PingMessage(), TestCancellationToken, TestTimeout, x =>
            {
                x.ConversationId = _conversationId;
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_two_handlers :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_actual_response()
        {
            (_, Task<Response<PingNotSupported>> notSupported) = await _request;

            Response<PingNotSupported> message = await notSupported;

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        [Test]
        public async Task Should_not_complete_the_handler()
        {
            (_, Task<Response<PingNotSupported>> notSupported) = await _request;

            Response<PingNotSupported> message = await notSupported;

            await BusSendEndpoint.Send(new PongMessage((await _ping).Message.CorrelationId));

            (Task<Response<PingMessage>> completed, _) = await _request;

            Assert.That(async () => await completed, Throws.TypeOf<TaskCanceledException>());
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Response<PingMessage, PingNotSupported>> _request;
        #pragma warning restore NUnit1032

        [OneTimeSetUp]
        public async Task Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();

            IRequestClient<PingMessage> requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
            _request = requestClient.GetResponse<PingMessage, PingNotSupported>(new PingMessage(), TestCancellationToken);

            await _request;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PingNotSupported(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Publishing_a_request :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_response_on_the_handler()
        {
            Response<PongMessage> message = await _request;

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _request;
        #pragma warning restore NUnit1032

        [OneTimeSetUp]
        public async Task Setup()
        {
            _request = Bus.Request<PingMessage, PongMessage>(new PingMessage(), TestCancellationToken, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, x => x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_no_handler :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_a_request_timeout_exception_on_the_handler()
        {
            Assert.That(async () => await _request, Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_receive_a_request_timeout_exception_on_the_request()
        {
            Assert.That(async () =>
                {
                    Response<PongMessage> response = await _request;
                },
                Throws.TypeOf<RequestTimeoutException>());
        }

        #pragma warning disable NUnit1032
        Task<Response<PongMessage>> _request;
        #pragma warning restore NUnit1032

        [OneTimeSetUp]
        public async Task Setup()
        {
            _request = Bus.Request<PingMessage, PongMessage>(new PingMessage(), TestCancellationToken, RequestTimeout.After(s: 1));
        }
    }
}
