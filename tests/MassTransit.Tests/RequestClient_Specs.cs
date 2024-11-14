namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
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

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_ping.Result.Message.CorrelationId));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _ping;
        Task<Response<PongMessage>> _response;
        #pragma warning restore NUnit1032
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

        #pragma warning disable NUnit1032
        Task<Response<PongMessage>> _response;
        #pragma warning restore NUnit1032
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

            IRequestClient<PingMessage> requestClient = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(1));

            Task<Response<PongMessage>> response = requestClient.GetResponse<PongMessage>(new PingMessage());

            Assert.That(async () => await response, Throws.TypeOf<RequestTimeoutException>());

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Multiple(() =>
            {
                Assert.That(unhandledExceptions, Is.Empty);
                Assert.That(unobservedTaskExceptions, Is.Empty);
            });
        }
    }


    [TestFixture]
    [Explicit]
    public class Sending_a_request_using_mediator_to_a_missing_service_that_times_out :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_timeout_without_exceptions()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(x =>
            {
                x.Handler<PingMessage>(async context =>
                {
                });
            });

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

            IRequestClient<PingMessage> requestClient = mediator.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(1));

            Task<Response<PongMessage>> response = requestClient.GetResponse<PongMessage>(new PingMessage());

            Assert.That(async () => await response, Throws.TypeOf<RequestTimeoutException>());

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Multiple(() =>
            {
                Assert.That(unhandledExceptions, Is.Empty, "Unhandled");
                Assert.That(unobservedTaskExceptions, Is.Empty, "Unobserved");
            });
        }
    }


    [TestFixture]
    [Explicit]
    public class Sending_a_request_using_mediator_that_faults :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_observe_all_exceptions()
        {
            var mediator = MassTransit.Bus.Factory.CreateMediator(x =>
            {
            });

            List<object> unhandledExceptions = new List<object>();
            List<Exception> unobservedTaskExceptions = new List<Exception>();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                unhandledExceptions.Add(eventArgs.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                eventArgs.SetObserved();
                unobservedTaskExceptions.Add(eventArgs.Exception);
            };

            IRequestClient<PingMessage> requestClient = mediator.CreateRequestClient<PingMessage>(InputQueueAddress, TimeSpan.FromSeconds(1));

            Task<Response<PongMessage>> response = requestClient.GetResponse<PongMessage>(new PingMessage());

            Assert.That(async () => await response, Throws.TypeOf<RequestException>());

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Multiple(() =>
            {
                Assert.That(unhandledExceptions, Is.Empty, "Unhandled");
                Assert.That(unobservedTaskExceptions, Is.Empty, "Unobserved");
            });
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

        #pragma warning disable NUnit1032
        Task<Response<PongMessage>> _response;
        #pragma warning restore NUnit1032
        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();

            _response = _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ = Handler<PingMessage>(configurator, async _ => throw new InvalidOperationException("This is an expected test failure"));
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

        #pragma warning disable NUnit1032
        Task<Response<PongMessage>> _response;
        #pragma warning restore NUnit1032
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
            _ = Handler<PingMessage>(configurator, async x =>
            {
                await Task.Delay(2000);
                await x.RespondAsync(new PongMessage(x.Message.CorrelationId));
            });
        }
    }


    [TestFixture]
    public class Publishing_from_a_request_consumer
    {
        [Test]
        public async Task Should_not_include_the_request_id()
        {
            await using var provider = new ServiceCollection()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<PingMessage> client = harness.GetRequestClient<PingMessage>();

            Response<PongMessage> response = await client.GetResponse<PongMessage>(new PingMessage());

            Assert.That(response.RequestId, Is.Not.Null);

            IPublishedMessage<PingHandled> published = await harness.Published.SelectAsync<PingHandled>().First();

            Assert.That(published, Is.Not.Null);
            Assert.That(published.Context.RequestId, Is.Null);
        }


        class PingHandled
        {
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.Publish(new PingHandled());

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}
