namespace MassTransit.Tests.ContainerTests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    public class Using_MultiBus :
        InMemoryContainerTestFixture
    {
        public Using_MultiBus()
        {
            Task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            Task2 = GetTask<ConsumeContext<PingMessage>>();
        }

        TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> Task1 { get; }
        TaskCompletionSource<ConsumeContext<PingMessage>> Task2 { get; }

        IBusOne One => ServiceProvider.GetService<IBusOne>();
        IEnumerable<IHostedService> HostedServices => ServiceProvider.GetService<IEnumerable<IHostedService>>();

        [Test]
        public async Task Should_receive()
        {
            await One.Publish<SimpleMessageInterface>(new SimpleMessageClass("abc"));

            await Task1.Task;
            await Task2.Task;
        }

        [Test]
        public void Should_resolve_bus_declaration()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ServiceProvider.GetService<IBusOne>(), Is.Not.Null);
                Assert.That(ServiceProvider.GetService<IBusTwo>(), Is.Not.Null);
            });
        }

        [Test]
        public void Should_resolve_bus_instance()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ServiceProvider.GetService<IBusInstance<IBusOne>>(), Is.Not.Null);
                Assert.That(ServiceProvider.GetService<IBusInstance<IBusTwo>>(), Is.Not.Null);
            });
        }

        [Test]
        public async Task Should_support_request_client_on_bus_one()
        {
            IRequestClient<OneRequest> client = GetRequestClient<OneRequest>();

            await client.GetResponse<OneResponse>(new OneRequest());
        }

        [Test]
        public async Task Should_support_request_client_on_bus_two()
        {
            IRequestClient<TwoRequest> client = GetRequestClient<TwoRequest>();

            await client.GetResponse<TwoResponse>(new TwoRequest());
        }

        [Test]
        public async Task Should_support_request_client_on_default_bus()
        {
            IRequestClient<Request> client = GetRequestClient<Request>();

            await client.GetResponse<Response>(new Request());
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<RequestConsumer>();
            configurator.AddRequestClient<Request>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<RequestConsumer>(BusRegistrationContext);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection = base.ConfigureServices(collection);

            collection.AddSingleton(Task1);
            collection.AddSingleton(Task2);

            collection.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            collection.AddMassTransit<IBusTwo>(ConfigureTwo);

            return collection;
        }

        static void ConfigureOne(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer1>();
            configurator.AddConsumer<OneRequestConsumer>();
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-one/"));
                cfg.ConfigureEndpoints(context);
            });
            configurator.AddRequestClient<OneRequest>();
        }

        static void ConfigureTwo(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer2>();
            configurator.AddConsumer<TwoRequestConsumer>();
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-two/"));
                cfg.ConfigureEndpoints(context);
            });
            configurator.AddRequestClient<TwoRequest>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StartAsync(InMemoryTestHarness.TestCancellationToken)));
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StopAsync(InMemoryTestHarness.TestCancellationToken)));
        }


        class Consumer1 :
            IConsumer<SimpleMessageInterface>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _taskCompletionSource;

            public Consumer1(IPublishEndpoint publishEndpointDefault, IBusTwo publishEndpoint,
                TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish(new PingMessage());
            }
        }


        class Consumer2 :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _taskCompletionSource;

            public Consumer2(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        class RequestConsumer :
            IConsumer<Request>
        {
            public Task Consume(ConsumeContext<Request> context)
            {
                return context.RespondAsync(new Response());
            }
        }


        class OneRequestConsumer :
            IConsumer<OneRequest>
        {
            public Task Consume(ConsumeContext<OneRequest> context)
            {
                return context.RespondAsync(new OneResponse());
            }
        }


        class TwoRequestConsumer :
            IConsumer<TwoRequest>
        {
            public Task Consume(ConsumeContext<TwoRequest> context)
            {
                return context.RespondAsync(new TwoResponse());
            }
        }
    }


    public class Using_MultiBus_With_ConfigureEndpoint :
        InMemoryContainerTestFixture
    {
        int _busOneConfigured;
        int _busTwoConfigured;
        int _globalConfigured;

        IEnumerable<IHostedService> HostedServices => ServiceProvider.GetService<IEnumerable<IHostedService>>();

        [Test]
        public async Task Should_configure_endpoints_correctly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_busOneConfigured, Is.EqualTo(1));
                Assert.That(_busTwoConfigured, Is.EqualTo(1));
                Assert.That(_globalConfigured, Is.EqualTo(2));
            });
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection = base.ConfigureServices(collection);

            collection.AddMassTransit<IBusOne, BusOne>(ConfigureOne);
            collection.AddMassTransit<IBusTwo>(ConfigureTwo);
            collection.AddSingleton<IConfigureReceiveEndpoint>(new GlobalConfigureReceiveEndpoint(() => Interlocked.Increment(ref _globalConfigured)));

            return collection;
        }

        void ConfigureOne(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer1>();
            configurator.AddConfigureEndpointsCallback((_, _, _) => Interlocked.Increment(ref _busOneConfigured));
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-one/"));
                cfg.ConfigureEndpoints(context);
            });
        }

        void ConfigureTwo(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<Consumer2>();
            configurator.AddConfigureEndpointsCallback((_, _, _) => Interlocked.Increment(ref _busTwoConfigured));
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.Host(new Uri("loopback://bus-two/"));
                cfg.ConfigureEndpoints(context);
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StartAsync(InMemoryTestHarness.TestCancellationToken)));
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StopAsync(InMemoryTestHarness.TestCancellationToken)));
        }


        class GlobalConfigureReceiveEndpoint :
            IConfigureReceiveEndpoint
        {
            readonly Action _action;

            public GlobalConfigureReceiveEndpoint(Action action)
            {
                _action = action;
            }

            public void Configure(string name, IReceiveEndpointConfigurator configurator)
            {
                _action();
            }
        }


        class Consumer1 :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        class Consumer2 :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    public class Using_consume_context_correctly_with_components :
        InMemoryContainerTestFixture
    {
        [Test]
        public async Task Should_receive_in_bus_through_mediator_and_publish()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg => cfg.AddConsumer<MediatorPublishConsumer>())
                .AddMassTransitTestHarness(cfg => cfg.AddConsumer<BusConsumer>())
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var mediator = harness.Scope.ServiceProvider.GetRequiredService<IScopedMediator>();
            await mediator.Send(new Request());

            Assert.That(await harness.Published.Any<OneRequest>(), Is.True);

            IReceivedMessage<OneRequest> consumed = await harness.Consumed.SelectAsync<OneRequest>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed.Context.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/mediator")));
        }

        [Test]
        public async Task Should_receive_in_bus_through_mediator_and_send()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg => cfg.AddConsumer<MediatorSendConsumer>())
                .AddMassTransitTestHarness(cfg => cfg.AddConsumer<BusConsumer>().Endpoint(x => x.Name = "input-queue"))
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var mediator = harness.Scope.ServiceProvider.GetRequiredService<IScopedMediator>();
            await mediator.Send(new Request());

            Assert.That(await harness.Sent.Any<OneRequest>(), Is.True);

            IReceivedMessage<OneRequest> consumed = await harness.Consumed.SelectAsync<OneRequest>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed.Context.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/mediator")));
        }

        [Test]
        public async Task Should_receive_in_bus_through_mediator_and_publish_with_different_base_address()
        {
            var baseAddress = new Uri($"loopback://localhost/{Guid.NewGuid()}");
            await using var provider = new ServiceCollection()
                .AddMediator(baseAddress, cfg => cfg.AddConsumer<MediatorPublishConsumer>())
                .AddMassTransitTestHarness(cfg => cfg.AddConsumer<BusConsumer>())
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var mediator = harness.Scope.ServiceProvider.GetRequiredService<IScopedMediator>();
            await mediator.Send(new Request());

            Assert.That(await harness.Published.Any<OneRequest>(), Is.True);

            IReceivedMessage<OneRequest> consumed = await harness.Consumed.SelectAsync<OneRequest>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed.Context.SourceAddress, Is.EqualTo(new Uri($"{baseAddress}/mediator")));
        }

        [Test]
        public async Task Should_receive_in_bus_through_mediator_and_send_with_different_base_address()
        {
            var baseAddress = new Uri($"loopback://localhost/{Guid.NewGuid()}");
            await using var provider = new ServiceCollection()
                .AddMediator(baseAddress, cfg => cfg.AddConsumer<MediatorSendConsumer>())
                .AddMassTransitTestHarness(cfg => cfg.AddConsumer<BusConsumer>().Endpoint(x => x.Name = "input-queue"))
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            var mediator = harness.Scope.ServiceProvider.GetRequiredService<IScopedMediator>();
            await mediator.Send(new Request());

            Assert.That(await harness.Sent.Any<OneRequest>(), Is.True);

            IReceivedMessage<OneRequest> consumed = await harness.Consumed.SelectAsync<OneRequest>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed.Context.SourceAddress, Is.EqualTo(new Uri($"{baseAddress}/mediator")));
        }


        class MediatorPublishConsumer :
            IConsumer<Request>
        {
            readonly IPublishEndpoint _publishEndpoint;

            public MediatorPublishConsumer(IPublishEndpoint publishEndpoint)
            {
                _publishEndpoint = publishEndpoint;
            }

            public Task Consume(ConsumeContext<Request> context)
            {
                return _publishEndpoint.Publish(new OneRequest());
            }
        }


        class MediatorSendConsumer :
            IConsumer<Request>
        {
            readonly ISendEndpointProvider _sendEndpointProvider;

            public MediatorSendConsumer(ISendEndpointProvider sendEndpointProvider)
            {
                _sendEndpointProvider = sendEndpointProvider;
            }

            public async Task Consume(ConsumeContext<Request> context)
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:input-queue"));
                await endpoint.Send(new OneRequest());
            }
        }


        class BusConsumer :
            IConsumer<OneRequest>
        {
            public Task Consume(ConsumeContext<OneRequest> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    namespace Contracts
    {
        public class Request
        {
        }


        public class Response
        {
        }


        public class OneRequest
        {
        }


        public class OneResponse
        {
        }


        public class TwoRequest
        {
        }


        public class TwoResponse
        {
        }


        public interface IBusOne :
            IBus
        {
        }


        public class BusOne :
            BusInstance<IBusOne>,
            IBusOne
        {
            public BusOne(IBusControl busControl)
                : base(busControl)
            {
            }
        }


        public interface IBusTwo :
            IBus
        {
        }
    }
}
