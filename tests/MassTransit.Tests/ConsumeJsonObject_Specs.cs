namespace MassTransit.Tests
{
    using System;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class ConsumeJsonObject_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_json_object()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _completed.Task;
        }

        [SetUp]
        public void Setup()
        {
            _completed = GetTask<JsonObject>();
        }

        TaskCompletionSource<JsonObject> _completed;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<JsonObject>(async context =>
            {
                await Console.Out.WriteLineAsync($"Received the json object! {context.Message}");

                _completed.TrySetResult(context.Message);
            });
        }
    }
}
