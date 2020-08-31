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
            var formatter = SnakeCaseEndpointNameFormatter.Instance;

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("some_really_cool"));
        }

        [Test]
        public void Should_convert_to_snake_case_with_digits()
        {
            var formatter = SnakeCaseEndpointNameFormatter.Instance;

            var name = formatter.Consumer<OneOr2MessageConsumer>();

            Assert.That(name, Is.EqualTo("one_or2_message"));
        }

        [Test]
        public void Should_convert_to_snake_case_with_uppercase_ids()
        {
            var formatter = SnakeCaseEndpointNameFormatter.Instance;

            var name = formatter.Consumer<SomeSuperIDFormatConsumer>();

            Assert.That(name, Is.EqualTo("some_super_idformat"));
        }

        [Test]
        public void Should_include_the_namespace()
        {
            var formatter = new KebabCaseEndpointNameFormatter(true);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("mass-transit-tests-endpoint-name-specs-some-really-cool"));
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
