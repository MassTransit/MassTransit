namespace MassTransit.Tests.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class When_consumers_are_generic_classes
    {
        List<string> _endpointNames;

        [SetUp]
        public void A_formatter_derives_endpoint_names()
        {
            var formatter = DefaultEndpointNameFormatter.Instance;

            _endpointNames = new List<string>
            {
                formatter.Consumer<GenericConsumer<Msg1>>(),
                formatter.Consumer<GenericConsumer<Msg2>>(),
                formatter.Consumer<GenericConsumer<Msg3>>()
            };
        }

        [Test]
        public void Should_all_be_unique()
        {
            Assert.That(_endpointNames.Distinct().Count(), Is.EqualTo(_endpointNames.Count()));
        }

        [Test]
        public void Should_be_named_after_first_generic_parameter()
        {
            Assert.That(_endpointNames, Is.EqualTo(new[] {nameof(Msg1), nameof(Msg2), nameof(Msg3)}));
        }


        class GenericConsumer<TMessage> : IConsumer<TMessage>
            where TMessage : class
        {
            public Task Consume(ConsumeContext<TMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        class Msg1
        {
        }


        class Msg2
        {
        }


        class Msg3
        {
        }
    }
}
