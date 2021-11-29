namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_fault_during_message_processing :
        RabbitMqTestFixture
    {
        [Test]
        [Order(0)]
        public async Task Setup()
        {
            _pingMessage = new PingMessage();
            _pingMessage2 = new PingMessage();
            await InputQueueSendEndpoint.Send(_pingMessage, Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
            }));
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _errorHandler2;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();
        PingMessage _pingMessage;
        PingMessage _pingMessage2;
        Task<ConsumeContext<PingMessage>> _errorHandler2;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x, context => context.Message.CorrelationId == _pingMessage.CorrelationId);
                _errorHandler2 = Handled<PingMessage>(x, context => context.Message.CorrelationId == _pingMessage2.CorrelationId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context =>
            {
                throw new IntentionalTestException("We want to be bad, so bad");
            });
        }
    }
}
