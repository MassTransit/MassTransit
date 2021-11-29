namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class ConsumeJToken_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_jtoken()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _completed.Task;
        }

        [SetUp]
        public void Setup()
        {
            _completed = GetTask<JToken>();
        }

        TaskCompletionSource<JToken> _completed;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseNewtonsoftJsonSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<JToken>(async context =>
            {
                await Console.Out.WriteLineAsync($"Received the token! {context.Message}");

                _completed.TrySetResult(context.Message);
            });
        }
    }
}
