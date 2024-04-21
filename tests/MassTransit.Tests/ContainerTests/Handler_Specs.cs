namespace MassTransit.Tests.ContainerTests
{
    using System.Threading.Tasks;
    using HandlerContracts;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace HandlerContracts
    {
        public record MyMessage;


        public record MyRequest;


        public record MyResponse;
    }


    public class Using_the_new_handler_registration
    {
        [Test]
        public async Task Should_handle_the_message_as_a_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyMessage message) =>
                    {
                        Assert.That(message, Is.Not.Null);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_consume_context_as_a_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyMessage> context) =>
                    {
                        Assert.That(context, Is.Not.Null);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_message_and_argument()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyMessage message, IService service) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(message, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_consume_context_and_argument()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyMessage> context, IService service) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(context, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_message_and_arguments()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyMessage message, IService service, IService2 service2) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(message, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_consume_context_and_arguments()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyMessage> context, IService service, IService2 service2) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(context, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_message_and_arguments3()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddScoped<IService3, Service3>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyMessage message, IService service, IService2 service2, IService3 service3) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(message, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_consume_context_and_arguments3()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddScoped<IService3, Service3>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyMessage> context, IService service, IService2 service2, IService3 service3) =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(context, Is.Not.Null);
                            Assert.That(service, Is.Not.Null);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MyMessage());

            Assert.That(await harness.Consumed.Any<MyMessage>(), Is.True);
        }

        [Test]
        public async Task Should_handle_the_request_as_a_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyRequest request) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_as_a_consumer_with_custom_endpoint()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyRequest request) => new MyResponse())
                        .Endpoint(e => e.Name = "custom");
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());

            Assert.That(response.SourceAddress.GetEndpointName(), Is.EqualTo("custom"));
        }

        [Test]
        public async Task Should_handle_the_request_context_as_a_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyRequest> request) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_and_argument()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyRequest request, IService service) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_context_and_argument()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyRequest> request, IService service) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_and_arguments()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyRequest request, IService service, IService2 service2) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_context_and_arguments()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyRequest> request, IService service, IService2 service2) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_and_arguments3()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddScoped<IService3, Service3>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (MyRequest request, IService service, IService2 service2, IService3 service3) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }

        [Test]
        public async Task Should_handle_the_request_context_and_arguments3()
        {
            await using var provider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .AddScoped<IService2, Service2>()
                .AddScoped<IService3, Service3>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (ConsumeContext<MyRequest> request, IService service, IService2 service2, IService3 service3) => new MyResponse());
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<MyRequest> client = harness.GetRequestClient<MyRequest>();

            Response<MyResponse> response = await client.GetResponse<MyResponse>(new MyRequest());
        }


        interface IService
        {
        }


        class Service :
            IService
        {
        }


        interface IService2
        {
        }


        class Service2 :
            IService2
        {
        }


        interface IService3
        {
        }


        class Service3 :
            IService3
        {
        }
    }
}
