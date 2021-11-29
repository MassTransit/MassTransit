namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class A_serialization_exception :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.CorrelationId, Is.EqualTo(_correlationId));
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get(MessageHeaders.FaultMessage, (string)null), Is.EqualTo("This is fine, forcing death"));
        }

        [Test]
        public async Task Should_have_the_frank_header()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("Frank", (string)null), Is.EqualTo("Happy"));
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.DestinationAddress, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public async Task Should_have_the_original_fault_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.FaultAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ResponseAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.SourceAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null), Is.EqualTo("fault"));
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        [Test]
        public async Task Should_not_have_the_estelle_header()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("Estelle", (string)null), Is.Null);
        }

        [Test]
        public async Task Should_not_have_the_fault_exception_type()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get(MessageHeaders.FaultExceptionType, (string)null), Is.Null);
        }

        [Test]
        public async Task Should_not_have_the_host_machine_name()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get(MessageHeaders.Host.MachineName, (string)null), Is.Null);
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
                context.ResponseAddress = Bus.Address;
                context.FaultAddress = Bus.Address;
                context.Headers.Set("Frank", "Happy");
                context.Headers.Set("Estelle", "Sad");
            }));
        }

        protected override void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            bool TransportHeaderCondition(HeaderValue<string> headerValue)
            {
                return headerValue.Key != "Estelle";
            }

            configurator.AllowTransportHeader(TransportHeaderCondition);
        }

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.ConfigureConsumeTopology = false;
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => throw new SerializationException("This is fine, forcing death"));
        }
    }
}
