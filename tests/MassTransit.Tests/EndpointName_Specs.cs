namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using Definition;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class EndpointName_Specs
    {
        [Test]
        public void Should_convert_to_snake_case()
        {
            var formatter = new SnakeCaseEndpointNameFormatter();

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("some_really_cool"));
        }

        [Test]
        public void Should_convert_to_snake_case_with_digits()
        {
            var formatter = new SnakeCaseEndpointNameFormatter();

            var name = formatter.Consumer<OneOr2MessageConsumer>();

            Assert.That(name, Is.EqualTo("one_or2_message"));
        }

        [Test]
        public void Should_convert_to_snake_case_with_uppercase_ids()
        {
            var formatter = new SnakeCaseEndpointNameFormatter();

            var name = formatter.Consumer<SomeSuperIDFormatConsumer>();

            Assert.That(name, Is.EqualTo("some_super_idformat"));
        }


        class SomeReallyCoolConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }


        class SomeSuperIDFormatConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }


        class OneOr2MessageConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }
    }
}
