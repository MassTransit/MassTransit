namespace MassTransit.Tests
{
    namespace NoLog
    {
        using System;
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using Metadata;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture]
        [Explicit]
        public class An_excessive_fault_storm :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_not_explode_the_task_library()
            {
                var limit = 1000;
                for (var i = 0; i < limit; i++)
                    await InputQueueSendEndpoint.Send(new PingMessage());

                IReceivedMessage<Fault<PingMessage>>[] messages = _consumer.Received.Select<Fault<PingMessage>>().Take(limit).ToArray();

                Assert.AreEqual(limit, messages.Length);

                Assert.AreEqual(limit,
                    messages.Select(x => x.Context.Message.Exceptions[0].ExceptionType == TypeCache<IntentionalTestException>.ShortName).Count());
            }

            PingConsumer _consumer;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _consumer = new PingConsumer(TestTimeout);

                _consumer.Configure(configurator);

                configurator.Consumer<MessageConsumer>();
            }


            class PingConsumer :
                MultiTestConsumer
            {
                public PingConsumer(TimeSpan timeout)
                    : base(timeout)
                {
                    Consume<Fault<PingMessage>>();
                }
            }


            public class MessageConsumer : IConsumer<PingMessage>
            {
                public async Task Consume(ConsumeContext<PingMessage> context)
                {
                    await Task.Delay(100);

                    throw new IntentionalTestException("Time for crunchin'");
                }
            }
        }
    }
}
