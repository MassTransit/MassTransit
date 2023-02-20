namespace MassTransit.Tests.Configuration
{
    using System.Linq;
    using MassTransit.Configuration;
    using NUnit.Framework;
    using Saga;
    using Saga.Messages;


    public class When_a_saga_is_inspected
    {
        SagaConnector<SimpleSaga> _factory;

        [SetUp]
        public void A_consumer_with_consumes_all_interfaces_is_inspected()
        {
            _factory = new SagaConnector<SimpleSaga>();
        }

        [Test]
        public void Should_create_the_builder()
        {
            Assert.That(_factory, Is.Not.Null);
        }

        [Test]
        public void Should_have_three_subscription_types()
        {
            Assert.That(_factory.Connectors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Should_have_an_a()
        {
            Assert.That(_factory.Connectors.First().MessageType, Is.EqualTo(typeof(InitiateSimpleSaga)));
        }

        [Test]
        public void Should_have_a_b()
        {
            Assert.That(_factory.Connectors.Skip(1).First().MessageType, Is.EqualTo(typeof(CompleteSimpleSaga)));
        }

        [Test]
        public void Should_have_a_c()
        {
            Assert.That(_factory.Connectors.Skip(2).First().MessageType, Is.EqualTo(typeof(ObservableSagaMessage)));
        }
    }
}
