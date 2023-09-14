namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MessageTypeSubjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class MessageType_Specs
    {
        [Test]
        public async Task Should_not_allow_array_message_types_but_does()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<ArrayConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new[] { new PingMessage(), new PingMessage(), new PingMessage() });

            Assert.That(await harness.Consumed.Any<PingMessage[]>());
        }


        class ArrayConsumer :
            IConsumer<PingMessage[]>
        {
            public Task Consume(ConsumeContext<PingMessage[]> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class Using_the_message_urn_attribute
    {
        [Test]
        public async Task Should_be_respected_by_the_message_type()
        {
            Assert.That(MessageUrn.ForTypeString<SomeClassMessage>(), Is.EqualTo("urn:message:SomeMessage"));
        }

        [Test]
        public async Task Should_be_respected_by_the_message_type_as_an_array()
        {
            Assert.That(MessageUrn.ForTypeString<SomeClassMessage[]>(), Is.EqualTo("urn:message:SomeMessage[]"));
        }
    }


    namespace MessageTypeSubjects
    {
        [MessageUrn("SomeMessage")]
        class SomeClassMessage
        {
        }
    }
}
