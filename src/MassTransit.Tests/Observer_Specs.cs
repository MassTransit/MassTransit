namespace MassTransit.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_observer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await _requestClient.Request(new PingMessage());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;
        PingObserver _observer;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _observer = new PingObserver();
            configurator.Observer(_observer, x =>
                x.UseExecute(context => Console.WriteLine($"Observer: {TypeMetadataCache<PingObserver>.ShortName}")));
        }


        class PingObserver :
            IObserver<ConsumeContext<PingMessage>>
        {
            readonly ConcurrentBag<PingMessage> _received;

            public PingObserver()
            {
                _received = new ConcurrentBag<PingMessage>();
            }

            public void OnNext(ConsumeContext<PingMessage> context)
            {
                _received.Add(context.Message);

                context.Respond(new PongMessage(context.Message.CorrelationId));
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }
    }
}
