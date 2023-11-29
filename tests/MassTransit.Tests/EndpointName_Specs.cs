namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Configuration;
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

        [Test]
        public void Should_include_the_namespace_and_prefix()
        {
            var formatter = new KebabCaseEndpointNameFormatter("Dev", true);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("dev-mass-transit-tests-endpoint-name-specs-some-really-cool"));
        }

        [Test]
        public void Should_include_the_namespace_and_prefix_with_generic_consumer()
        {
            var formatter = new KebabCaseEndpointNameFormatter("Dev", true);

            var name = formatter.Consumer<SomeGenericConsumer<PingMessage>>();

            Assert.That(name, Is.EqualTo("dev-mass-transit-test-framework-messages-ping-message"));
        }

        [Test]
        public void Should_include_the_namespace_and_prefix_with_message_name()
        {
            var formatter = new KebabCaseEndpointNameFormatter("Dev", true);

            var name = formatter.Message<PingMessage>();

            Assert.That(name, Is.EqualTo("dev-mass-transit-test-framework-messages-ping-message"));
        }

        [Test]
        public void Should_only_include_the_message_name()
        {
            var name = KebabCaseEndpointNameFormatter.Instance.Message<PingMessage>();

            Assert.That(name, Is.EqualTo("ping-message"));
        }

        [Test]
        public void Should_include_the_prefix()
        {
            var formatter = new KebabCaseEndpointNameFormatter("Dev", false);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("dev-some-really-cool"));
        }

        [Test]
        public void Should_include_the_prefix_default()
        {
            var formatter = new DefaultEndpointNameFormatter("Dev", false);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("DevSomeReallyCool"));
        }

        [Test]
        public void Should_include_the_prefix_default_with_separator()
        {
            var formatter = new DefaultEndpointNameFormatter("Dev-", false);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("Dev-SomeReallyCool"));
        }

        [Test]
        public void Should_include_the_prefix_snake_with_separator()
        {
            var formatter = new SnakeCaseEndpointNameFormatter("Dev-", false);

            var name = formatter.Consumer<SomeReallyCoolConsumer>();

            Assert.That(name, Is.EqualTo("dev-some_really_cool"));
        }

        [Test]
        public void Should_format_instance_id_properly()
        {
            var endpointSettings = new EndpointSettings<IEndpointDefinition<SomeReallyCoolConsumer>> {InstanceId = "TopShelf"};
            var endpointDefinition = new ConsumerEndpointDefinition<SomeReallyCoolConsumer>(endpointSettings);

            var name = endpointDefinition.GetEndpointName(KebabCaseEndpointNameFormatter.Instance);

            Assert.That(name, Is.EqualTo("some-really-cool-top-shelf"));
        }

        [Test]
        public void Should_throw_exception_when_class_is_called_consumer()
        {
            var formatter = DefaultEndpointNameFormatter.Instance;

            var exception = Assert.Throws<ConfigurationException>(() => formatter.Consumer<Consumer>());
            Assert.AreEqual(
                "You cannot have a consumer named \"Consumer\" - please add a meaningful prefix. MassTransit will automatically remove the \"Consumer\" suffix from your type names when you do not specify a dedicated name.",
                exception.Message
            );
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

        class SomeGenericConsumer<T> :
            IConsumer<T>
            where T : class
        {
            public async Task Consume(ConsumeContext<T> context)
            {
            }
        }


        class Consumer : IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }
    }
}
