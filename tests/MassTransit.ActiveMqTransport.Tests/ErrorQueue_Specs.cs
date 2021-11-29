namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Metadata;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class A_serialization_exception :
        ActiveMqTestFixture
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

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null), Is.EqualTo("This is fine, forcing death"));
        }

        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Host-MachineName", (string)null), Is.EqualTo(HostMetadataCache.Host.MachineName));
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
            }));
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.ConfigureConsumeTopology = false;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }
}
