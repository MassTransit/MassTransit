namespace MassTransit.Tests
{
    namespace MyNamespace
    {
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using MassTransit.Testing.Implementations;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture]
        [Category("Unit")]
        public class Observing_consumer_messages :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_trigger_the_consume_message_observer()
            {
                var context = await _pingObserver.PostConsumed;
            }

            [Test]
            public void Should_trigger_the_consume_observer()
            {
                IReceivedMessage<PingMessage> context = _observer.Messages.Select<PingMessage>().First();

                context.ShouldNotBeNull();
            }

            TestConsumeMessageObserver<PingMessage> _pingObserver;
            TestConsumeObserver _observer;

            [OneTimeSetUp]
            public async Task SetupObservers()
            {
                _pingObserver = GetConsumeObserver<PingMessage>();
                Bus.ConnectConsumeMessageObserver(_pingObserver);

                _observer = GetConsumeObserver();
                Bus.ConnectConsumeObserver(_observer);

                await Bus.Publish(new PingMessage());
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                Handled<PingMessage>(configurator);
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class Observing_consumer_messages_with_mediator :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_trigger_the_consume_message_observer()
            {
                var observer = GetConsumeObserver();
                TestConsumeMessageObserver<PingMessage> pingObserver = GetConsumeObserver<PingMessage>();

                var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
                {
                });

                mediator.ConnectConsumeObserver(observer);
                mediator.ConnectConsumeMessageObserver(pingObserver);

                TaskCompletionSource<ConsumeContext<PingMessage>> received = GetTask<ConsumeContext<PingMessage>>();

                var handle = mediator.ConnectHandler<PingMessage>(x =>
                {
                    received.SetResult(x);

                    return Task.CompletedTask;
                });

                await mediator.Publish(new PingMessage());

                await received.Task;

                handle.Disconnect();

                await pingObserver.PostConsumed;

                IReceivedMessage<PingMessage> context = observer.Messages.Select<PingMessage>().First();

                context.ShouldNotBeNull();
            }

            [Test]
            public async Task Should_trigger_the_consume_message_observer_for_both()
            {
                var observer = GetConsumeObserver();
                TestConsumeMessageObserver<PingMessage> pingObserver = GetConsumeObserver<PingMessage>();

                var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
                {
                });

                mediator.ConnectConsumeObserver(observer);
                mediator.ConnectConsumeMessageObserver(pingObserver);

                TaskCompletionSource<ConsumeContext<PingMessage>> received = GetTask<ConsumeContext<PingMessage>>();

                var handle = mediator.ConnectHandler<PingMessage>(x =>
                {
                    received.SetResult(x);

                    return x.RespondAsync(new PongMessage(x.Message.CorrelationId));
                });

                IRequestClient<PingMessage> client = mediator.CreateRequestClient<PingMessage>();

                var pingMessage = new PingMessage();

                Response<PongMessage> response = await client.GetResponse<PongMessage>(pingMessage);

                await received.Task;

                handle.Disconnect();

                await pingObserver.PostConsumed;

                Assert.That(observer.Messages.Select<PingMessage>().First(), Is.Not.Null);
                Assert.That(observer.Messages.Select<PongMessage>().First(), Is.Not.Null);
            }
        }
    }
}
