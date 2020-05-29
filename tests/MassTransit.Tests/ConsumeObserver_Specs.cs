namespace MassTransit.Tests
{
    namespace MyNamespace
    {
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using MassTransit.Testing.Observers;
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
    }
}
