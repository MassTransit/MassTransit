namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_publishing_an_interface_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_address_value()
        {
            ConsumeContext<IProxyMe> message = await _handler;

            Assert.That(message.Message.Address.OriginalString, Is.EqualTo(UriString));
        }

        [Test]
        public async Task Should_have_correlation_id()
        {
            ConsumeContext<IProxyMe> message = await _handler;

            Assert.That(message.Message.CorrelationId, Is.EqualTo(_correlationId));
        }

        [Test]
        public async Task Should_have_integer_value()
        {
            ConsumeContext<IProxyMe> message = await _handler;

            Assert.That(message.Message.IntValue, Is.EqualTo(IntValue));
        }

        [Test]
        public async Task Should_have_received_message()
        {
            await _handler;
        }

        [Test]
        public async Task Should_have_string_value()
        {
            ConsumeContext<IProxyMe> message = await _handler;

            Assert.That(message.Message.StringValue, Is.EqualTo(StringValue));
        }

        const int IntValue = 42;
        const string StringValue = "Hello";
        readonly Guid _correlationId = Guid.NewGuid();
        #pragma warning disable NUnit1032
        Task<ConsumeContext<IProxyMe>> _handler;
        #pragma warning restore NUnit1032
        const string UriString = "http://localhost/";

        [OneTimeSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send<IProxyMe>(new
                {
                    IntValue,
                    StringValue,
                    Address = new Uri(UriString),
                    CorrelationId = _correlationId
                })
                .Wait(TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<IProxyMe>(configurator);
        }


        public interface IProxyMe :
            CorrelatedBy<Guid>
        {
            int IntValue { get; }
            string StringValue { get; }
            Uri Address { get; }
        }
    }
}
