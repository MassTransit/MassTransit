namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class EventPublishRQ_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_publish_first_event()
        {
            ConsumeContext<PingReceived> received = await _received;
        }

        [Test]
        public async Task Should_publish_second_event()
        {
            ConsumeContext<PingProcessing> consumed = await _processing;
        }

        [Test]
        public async Task Should_publish_third_event()
        {
            ConsumeContext<PingConsumed> consumed = await _consumed;
        }

        Task<ConsumeContext<PingReceived>> _received;
        Task<ConsumeContext<PingConsumed>> _consumed;
        Task<ConsumeContext<PingProcessing>> _processing;

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumar>();

            _received = Handled<PingReceived>(configurator);
            _processing = Handled<PingProcessing>(configurator);
            _consumed = Handled<PingConsumed>(configurator);
        }


        class Consumar :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.Publish<PingReceived>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                Console.WriteLine("Ping: {0}", context.Message.CorrelationId);

                await context.Publish<PingProcessing>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                Console.WriteLine("Prcessing: {0}", context.Message.CorrelationId);

                await context.Publish<PingConsumed>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }


        public interface PingReceived
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }


        public interface PingProcessing
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }


        public interface PingConsumed
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }
    }
}
