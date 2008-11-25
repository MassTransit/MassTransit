namespace MassTransit.Tests
{
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using ServiceBus.Transports;

    [TestFixture]
    public class When_creating_a_service_bus
    {
        [Test]
        public void I_want_an_easy_to_understand_builder_to_set_it_up_for_me()
        {
            const string endpointUrl = "loopback://localhost/test_servicebus";

            ServiceBus bus = ServiceBus.Build()
                .SupportingTransport<LoopbackEndpoint>()
                .ListeningOn(endpointUrl);

            Assert.That(bus.Endpoint.Uri.ToString(), Is.EqualTo(endpointUrl));
        }
    }
}