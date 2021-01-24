namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<PongMessage> message = await _response;

            message.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        InMemoryTestFixture
    {
        [Test]
        public void Should_timeout()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());
        }

        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }
    }


    [TestFixture]
    [Explicit]
    public class Sending_a_request_to_a_missing_service_that_times_out :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_timeout_without_exceptions()
        {
            List<object> unhandledExceptions = new List<object>();
            List<Exception> unobservedTaskExceptions = new List<Exception>();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                unhandledExceptions.Add(eventArgs.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                unobservedTaskExceptions.Add(eventArgs.Exception);
            };

            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.That(unhandledExceptions, Is.Empty);
            Assert.That(unobservedTaskExceptions, Is.Empty);
        }

        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        InMemoryTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }


    [TestFixture]
    public class Cancelling_a_request_mid_stream :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_throw_a_cancelled_exception()
        {
            Assert.That(async () => await _response, Throws.TypeOf<TaskCanceledException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();

            var cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await Task.Delay(500);
                cts.Cancel();
            });

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage(), cts.Token);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                await Task.Delay(2000);
                await x.RespondAsync(new PongMessage(x.Message.CorrelationId));
            });
        }
    }
}
