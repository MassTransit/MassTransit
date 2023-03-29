namespace MassTransit.Tests.Topology
{
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TopologyTestSubjects;


    namespace TopologyTestSubjects
    {
        public interface JsonMessage
        {
            string Value { get; }
        }
    }


    [TestFixture]
    public class Specifying_a_message_serializer_via_topology
    {
        [Test]
        public async Task Should_use_the_serializer_for_messages()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddHandler(async (JsonMessage message) =>
                    {
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.AddRawJsonSerializer();

                        cfg.Send<JsonMessage>(m => m.UseSerializer("application/json"));

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<JsonMessage>(new { Value = "Frank" });

            Assert.That(await harness.Consumed.Any<JsonMessage>(), Is.True);

            IReceivedMessage<JsonMessage> received = await harness.Consumed.SelectAsync<JsonMessage>().First();

            Assert.That(received.Context.ReceiveContext.ContentType, Is.EqualTo(SystemTextJsonRawMessageSerializer.JsonContentType));
        }
    }
}
