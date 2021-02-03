namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using MultiBus;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public abstract class Common_MultiBus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive()
        {
            await One.Publish<SimpleMessageInterface>(new SimpleMessageClass("abc"));

            await Task1.Task;
            await Task2.Task;
        }

        [Test]
        public async Task Should_support_request_client_on_default_bus()
        {
            IRequestClient<Request> client = GetRequestClient<Request>();

            await client.GetResponse<Response>(new Request());
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

        protected readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> Task1;
        protected readonly TaskCompletionSource<ConsumeContext<PingMessage>> Task2;
        protected abstract IBusOne One { get; }
        protected abstract IEnumerable<IHostedService> HostedServices { get; }

        protected abstract IRequestClient<T> GetRequestClient<T>()
            where T : class;

        protected static void ConfigureRegistration(IRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<RequestConsumer>();
            configurator.AddRequestClient<Request>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<RequestConsumer>(Registration);
        }

        protected abstract IBusRegistrationContext Registration { get; }

        protected static void ConfigureOne(IBusRegistrationConfigurator configurator)
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

        protected static void ConfigureTwo(IBusRegistrationConfigurator configurator)
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

        protected Common_MultiBus()
        {
            Task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            Task2 = GetTask<ConsumeContext<PingMessage>>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StartAsync(TestCancellationToken)));
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Task.WhenAll(HostedServices.Select(x => x.StopAsync(TestCancellationToken)));
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


        class RequestConsumer :
            IConsumer<Request>
        {
            public Task Consume(ConsumeContext<Request> context)
            {
                return context.RespondAsync(new Response());
            }
        }


        public class Request
        {
        }


        public class Response
        {
        }


        class OneRequestConsumer :
            IConsumer<OneRequest>
        {
            public Task Consume(ConsumeContext<OneRequest> context)
            {
                return context.RespondAsync(new OneResponse());
            }
        }


        public class OneRequest
        {
        }


        public class OneResponse
        {
        }


        class TwoRequestConsumer :
            IConsumer<TwoRequest>
        {
            public Task Consume(ConsumeContext<TwoRequest> context)
            {
                return context.RespondAsync(new TwoResponse());
            }
        }


        public class TwoRequest
        {
        }


        public class TwoResponse
        {
        }
    }
}
