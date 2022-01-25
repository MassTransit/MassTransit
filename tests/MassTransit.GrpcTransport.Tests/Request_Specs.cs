namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;
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

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client_via_new_endpoint_name :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(new Uri("exchange:input-queue"), TestTimeout);

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client_in_a_consumer :
        GrpcTestFixture
    {
        [Test]
        [Order(0)]
        public void Get_response()
        {
            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        [Test]
        [Order(2)]
        public async Task Should_have_the_conversation_id()
        {
            ConsumeContext<PingMessage> ping = await _ping;
            ConsumeContext<A> a = await _a;
        }

        [Test]
        [Order(1)]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            Assert.That(message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IClientFactory _clientFactory;
        IRequestClient<PingMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _clientFactory = Bus.CreateClientFactory();

            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_clientFactory is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                IRequestClient<A> client = _clientFactory.CreateRequestClient<A>(x, InputQueueAddress);

                await client.GetResponse<B>(new A(), context => context.TimeToLive = TimeSpan.FromSeconds(30), x.CancellationToken);

                await x.RespondAsync(new PongMessage(x.Message.CorrelationId));
            });

            _a = Handler<A>(configurator, x => x.RespondAsync(new B()));
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client_in_a_consumer_also :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_have_the_conversation_id()
        {
            ConsumeContext<PingMessage> ping = await _ping;
            ConsumeContext<A> a = await _a;

            Assert.That(ping.ConversationId, Is.EqualTo(a.ConversationId));
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IClientFactory _clientFactory;
        IRequestClient<PingMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _clientFactory = await Bus.ConnectClientFactory(TestTimeout);

            _requestClient = _clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_clientFactory is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                RequestHandle<A> request = _clientFactory.CreateRequest(x, InputQueueAddress, new A(), x.CancellationToken);

                await request.GetResponse<B>();

                await x.RespondAsync(new PongMessage(x.Message.CorrelationId));
            });

            _a = Handler<A>(configurator, x => x.RespondAsync(new B()));
        }


        class A
        {
        }


        class B
        {
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        GrpcTestFixture
    {
        [Test]
        public void Should_timeout()
        {
            Assert.That(async () => await _requestClient.GetResponse<PongMessage>(new PingMessage()), Throws.TypeOf<RequestTimeoutException>());
        }

        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(4));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        GrpcTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _requestClient.GetResponse<PongMessage>(new PingMessage()), Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => throw new InvalidOperationException("This is an expected test failure"));
        }
    }
}
