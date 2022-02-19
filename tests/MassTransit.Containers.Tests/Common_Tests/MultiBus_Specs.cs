namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;
    using Transports;


    [TestFixture(typeof(DependencyInjectionTestFixtureContainerFactory))]
    public class Using_MultiBus<TContainer> :
        InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
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
            Assert.NotNull(ServiceProvider.GetService<IBusOne>());
            Assert.NotNull(ServiceProvider.GetService<IBusTwo>());
        }

        [Test]
        public void Should_resolve_bus_instance()
        {
            Assert.NotNull(ServiceProvider.GetService<IBusInstance<IBusOne>>());
            Assert.NotNull(ServiceProvider.GetService<IBusInstance<IBusTwo>>());
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

        TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> Task1 { get; }
        TaskCompletionSource<ConsumeContext<PingMessage>> Task2 { get; }

        IBusOne One => ServiceProvider.GetService<IBusOne>();
        IEnumerable<IHostedService> HostedServices => ServiceProvider.GetService<IEnumerable<IHostedService>>();

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

        public Using_MultiBus()
        {
            Task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            Task2 = GetTask<ConsumeContext<PingMessage>>();
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
