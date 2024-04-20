namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using MultiBusMessages;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_request_client_across_bus_instances
    {
        [Test]
        public async Task Should_handle_responses_properly()
        {
            await using var provider = new ServiceCollection()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddRequestClient<RequestA>();
                    x.AddConsumer<ConsumerA>();
                })
                .AddMassTransit<IBusB>(x =>
                {
                    x.AddRequestClient<RequestB>(RequestTimeout.After(s: 2));
                    x.AddConsumer<ConsumerB>();

                    x.UsingInMemory((context, configurator) =>
                    {
                        configurator.Host(new Uri("loopback://localhost/b"));

                        configurator.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<RequestA> aClient = harness.GetRequestClient<RequestA>();

            Response<ResponseA> response = await aClient.GetResponse<ResponseA>(new
            {
                Key = "Hello",
                __Header_Test_Value = "Hello, World"
            }, timeout: harness.TestInactivityTimeout);

            Assert.Multiple(() =>
            {
                Assert.That(response.Message.Value, Is.EqualTo("Key: Hello"));
                Assert.That(response.Headers.Get<string>("Test-Value"), Is.EqualTo("Hello, World"));
            });
        }


        class ConsumerA :
            IConsumer<RequestA>
        {
            readonly IRequestClient<RequestB> _client;

            public ConsumerA(IRequestClient<RequestB> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<RequestA> context)
            {
                Response<ResponseB> response = await _client.GetResponse<ResponseB>(context.Message);

                await context.RespondAsync<ResponseA>(response.Message);
            }
        }


        class ConsumerB :
            IConsumer<RequestB>
        {
            public async Task Consume(ConsumeContext<RequestB> context)
            {
                await context.RespondAsync<ResponseB>(new { Value = $"Key: {context.Message.Key}" });
            }
        }
    }


    namespace MultiBusMessages
    {
        public interface RequestA
        {
            string Key { get; }
        }


        public interface ResponseA
        {
            string Value { get; }
        }


        public interface RequestB
        {
            string Key { get; }
        }


        public interface ResponseB
        {
            string Value { get; }
        }


        public interface IBusB :
            IBus
        {
        }
    }
}
