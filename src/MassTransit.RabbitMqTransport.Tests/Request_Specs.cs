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
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
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
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8), context => context.SetAwaitAck(false));

            _response = _requestClient.Request(new PingMessage());
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
            _response = _requestClient.Request(new PingMessage());

            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
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
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8), context =>
                {
                    context.SetAwaitAck(false);
                    context.ResponseAddress = new UriBuilder(Bus.Address) {Host = "totally-bogus-host"}.Uri;
                });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_the_broker_down_using_the_request_client :
        RabbitMqTestFixture
    {
        [Test, Explicit, Category("SlowAF")]
        public async Task Should_receive_the_response()
        {
            await Task.Delay(15000);

            var response = _requestClient.Request(new PingMessage());

            Assert.That(async () => await response, Throws.TypeOf<RequestException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreatePublishRequestClient<PingMessage, PongMessage>(TestTimeout);
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
            var message = await _response;

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
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, RequestTimeout.After(s: 8));

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
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(new Uri("exchange:input_queue"), RequestTimeout.After(s: 8));

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client_in_a_consumer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_have_the_conversation_id()
        {
            var ping = await _ping;
            var a = await _a;

            ping.ConversationId.ShouldBe(a.ConversationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClientFactory<A, B> _factory;
        IRequestClient<PingMessage, PongMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _factory = await Host.CreateRequestClientFactory<A, B>(InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));

            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _factory.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                var client = _factory.CreateRequestClient(x);
                await client.Request(new A(), x.CancellationToken);

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
    public class Sending_a_request_using_the_new_request_client_in_a_consumer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_have_the_conversation_id()
        {
            var ping = await _ping;
            var a = await _a;

            ping.ConversationId.ShouldBe(a.ConversationId);
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

            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, RequestTimeout.After(s: 8));

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _clientFactory.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                var client = _clientFactory.CreateRequestClient<A>(x, InputQueueAddress);

                var request = client.Create(new A(), x.CancellationToken);

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
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_have_the_conversation_id()
        {
            var ping = await _ping;
            var a = await _a;

            ping.ConversationId.ShouldBe(a.ConversationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IClientFactory _clientFactory;
        IRequestClient<PingMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _clientFactory = await Host.CreateClientFactory(RequestTimeout.After(s: 8));

            _requestClient = await Host.CreateRequestClient<PingMessage>(InputQueueAddress, RequestTimeout.After(s: 8));

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _clientFactory.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                var request = _clientFactory.CreateRequest(x, InputQueueAddress, new A(), x.CancellationToken);

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
            var message = await _requestClient.Request(new PingMessage());

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));
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
            Assert.That(async () => await _requestClient.Request(new PingMessage()), Throws.TypeOf<RequestTimeoutException>());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(1));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _requestClient.Request(new PingMessage()), Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8));
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }

    [TestFixture]
    public class Sending_a_request_to_a_faulty_service_using_reply_to :
        RabbitMqTestFixture
    {
        [Test]
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

            _requestClient = _clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(8));
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _clientFactory.DisposeAsync();
        }


        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }
}
