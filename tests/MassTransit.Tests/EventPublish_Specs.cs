namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class EventPublish_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_first_event()
        {
            ConsumeContext<PingReceived> received = await _received;
        }

        [Test]
        public async Task Should_publish_fourth_event()
        {
            ConsumeContext<PingCompleted> consumed = await _completed;
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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingReceived>> _received;
        Task<ConsumeContext<PingConsumed>> _consumed;
        Task<ConsumeContext<PingCompleted>> _completed;
        Task<ConsumeContext<PingProcessing>> _processing;
        #pragma warning restore NUnit1032

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new Consumar());

            _received = Handled<PingReceived>(configurator);
            _processing = Handled<PingProcessing>(configurator);
            _consumed = Handled<PingConsumed>(configurator);
            _completed = Handled<PingCompleted>(configurator);
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

                Console.WriteLine("Processing: {0}", context.Message.CorrelationId);

                await context.Publish<PingConsumed>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                await context.Publish<PingCompleted>(new
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


        public interface PingCompleted
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }
    }
}
