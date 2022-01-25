namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            using RequestHandle<PingMessage> requestHandle = _requestClient.Create(new PingMessage());

            requestHandle.UseExecute(context => context.SetAwaitAck(false));

            Response<PongMessage> response = await requestHandle.GetResponse<PongMessage>();

            response.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        public Sending_a_request_using_the_request_client()
        {
            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureHost;
        }

        void ConfigureHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.PublisherConfirmation = false;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client_with_raw_json :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            using RequestHandle<PingMessage> requestHandle = _requestClient.Create(new PingMessage());

            requestHandle.UseExecute(context => context.SetAwaitAck(false));

            Response<PongMessage> response = await requestHandle.GetResponse<PongMessage>();

            response.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            base.ConfigureRabbitMqBus(configurator);

            configurator.UseRawJsonSerializer();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(TestTimeout);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_a_different_host_name :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            using RequestHandle<PingMessage> requestHandle = _requestClient.Create(new PingMessage());

            requestHandle.UseExecute(context =>
            {
                context.SetAwaitAck(false);
                context.ResponseAddress = new UriBuilder(Bus.Address) { Host = "totally-bogus-host" }.Uri;
            });

            Response<PongMessage> response = await requestHandle.GetResponse<PongMessage>();

            response.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        public Sending_a_request_with_a_different_host_name()
        {
            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureHost;
        }

        void ConfigureHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.PublisherConfirmation = false;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        public Sending_a_request_using_the_new_request_client()
        {
            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureHost;
        }

        void ConfigureHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.PublisherConfirmation = false;
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

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client_via_new_endpoint_name :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(new Uri("exchange:input_queue"), TestTimeout);

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_new_request_client_in_a_consumer :
        RabbitMqTestFixture
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

            ping.ConversationId.ShouldBe(a.ConversationId);
        }

        [Test]
        [Order(1)]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IClientFactory _clientFactory;
        IRequestClient<PingMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _clientFactory = await Bus.CreateReplyToClientFactory();

            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_clientFactory is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                IRequestClient<A> client = _clientFactory.CreateRequestClient<A>(x, InputQueueAddress);

                RequestHandle<A> request = client.Create(new A(), x.CancellationToken);

                await request.GetResponse<B>();

                x.Respond(new PongMessage(x.Message.CorrelationId));
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
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_conversation_id()
        {
            ConsumeContext<PingMessage> ping = await _ping;
            ConsumeContext<A> a = await _a;

            ping.ConversationId.ShouldBe(a.ConversationId);
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
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

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                RequestHandle<A> request = _clientFactory.CreateRequest(x, InputQueueAddress, new A(), x.CancellationToken);

                await request.GetResponse<B>();

                x.Respond(new PongMessage(x.Message.CorrelationId));
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
    public class Sending_a_request_using_the_request_client_with_no_confirmations :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> response = await _requestClient.GetResponse<PongMessage>(new PingMessage());

            response.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        RabbitMqTestFixture
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
        RabbitMqTestFixture
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

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => throw new InvalidOperationException("This is an expected test failure"));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service_using_reply_to :
        RabbitMqTestFixture
    {
        [Test]
        [Retry(3)]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _requestClient.GetResponse<PongMessage>(new PingMessage()), Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;
        IClientFactory _clientFactory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _clientFactory = await Bus.CreateReplyToClientFactory();

            _requestClient = _clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_clientFactory is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => throw new InvalidOperationException("This is an expected test failure"));
        }
    }
}
