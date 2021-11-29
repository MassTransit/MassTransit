namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_messages_through_the_outbox :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_receive_the_response()
        {
            Task<ConsumeContext<PongMessage>> responseHandler = SubscribeHandler<PongMessage>();

            Assert.That(async () =>
                {
                    Response<PongMessage> response = await Bus.Request<PingMessage, PongMessage>(InputQueueAddress, new PingMessage(), TestCancellationToken,
                        RequestTimeout.After(s: 3));
                },
                Throws.TypeOf<RequestFaultException>());

            await _pingReceived.Task;

            Console.WriteLine("Ping was received");

            Assert.That(
                async () => await responseHandler.OrCanceled(new CancellationTokenSource(300).Token),
                Throws.TypeOf<OperationCanceledException>());
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            _pingReceived = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(context =>
            {
                _pingReceived.TrySetResult(context);

                context.Respond(new PongMessage(context.Message.CorrelationId));

                throw new IntentionalTestException("This time bad things happen");
            });
        }
    }
}
