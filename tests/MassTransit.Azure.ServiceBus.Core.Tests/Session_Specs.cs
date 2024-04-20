namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_session :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_have_a_redelivery_flag_of_false()
        {
            ConsumeContext<PingMessage> context = await _handler;

            Assert.That(context.ReceiveContext.Redelivered, Is.False);
        }

        [Test]
        public async Task Should_receive_published_message()
        {
            ConsumeContext<PingReceived> context = await _received;

            Assert.That(context.SessionId(), Is.EqualTo(_correlationId.ToString("D")));
        }

        [Test]
        public async Task Should_receive_sent_message()
        {
            ConsumeContext<PingAccepted> context = await _accepted;

            Assert.That(context.SessionId(), Is.EqualTo(_correlationId.ToString("D")));
        }

        [Test]
        public async Task Should_succeed()
        {
            ConsumeContext<PingMessage> context = await _handler;

            var sessionId = await _handlerSessionId;

            Assert.That(sessionId, Is.EqualTo(_correlationId.ToString("D")));
        }

        public Sending_a_message_to_a_session()
            : base("input_queue_session")
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        Task<ConsumeContext<PingMessage>> _handler;
        Task<ConsumeContext<PingAccepted>> _accepted;
        Task<ConsumeContext<PingReceived>> _received;
        Task<string> _handlerSessionId;
        Guid _correlationId;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RequiresSession = true;

            TaskCompletionSource<string> handlerSessionId = GetTask<string>();
            _handlerSessionId = handlerSessionId.Task;

            _handler = Handler<PingMessage>(configurator, async context =>
            {
                await context.Publish<PingReceived>(context.Message);
                await context.Send<PingAccepted>(InputQueueAddress, context.Message);

                handlerSessionId.TrySetResult(context.SessionId());
            });

            _accepted = Handled<PingAccepted>(configurator);
            _received = Handled<PingReceived>(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var message = new PingMessage();
            _correlationId = message.CorrelationId;

            await InputQueueSendEndpoint.Send(message, context =>
            {
                context.SetSessionId(message.CorrelationId.ToString("D"));
            });
        }


        public interface PingReceived
        {
            Guid CorrelationId { get; }
        }


        public interface PingAccepted
        {
            Guid CorrelationId { get; }
        }
    }
}
