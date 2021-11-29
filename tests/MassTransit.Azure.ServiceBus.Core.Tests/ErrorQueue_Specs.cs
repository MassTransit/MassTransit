namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class A_serialization_exception :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.CorrelationId.ShouldBe(_correlationId);
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null).ShouldBe("This is fine, forcing death");
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.DestinationAddress.ShouldBe(InputQueueAddress);
        }

        [Test]
        public async Task Should_have_the_original_fault_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.FaultAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ResponseAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null).ShouldBe("fault");
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
                context.ResponseAddress = context.SourceAddress;
                context.FaultAddress = context.SourceAddress;
            }));
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.ConfigureConsumeTopology = false;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }
}
