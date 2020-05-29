namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Testing;
    using MassTransit.Testing.MessageObservers;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class A_faulting_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_a_single_fault_when_retried()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            _consumer.Faults.Select().Count().ShouldBe(1);
        }

        PingConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new PingConsumer(TimeSpan.FromSeconds(5), InMemoryTestHarness.InactivityToken);

            configurator.UseRetry(r => r.Immediate(5));
            _consumer.Configure(configurator);
        }


        class PingConsumer :
            MultiTestConsumer
        {
            readonly ReceivedMessageList<Fault<PingMessage>> _faults;
            readonly ReceivedMessageList<PingMessage> _messages;

            public PingConsumer(TimeSpan timeout, CancellationToken testCompleted)
                : base(timeout, testCompleted)
            {
                _messages = Fault<PingMessage>();
                _faults = Consume<Fault<PingMessage>>();
            }

            public IReceivedMessageList<PingMessage> Messages => _messages;
            public IReceivedMessageList<Fault<PingMessage>> Faults => _faults;
        }
    }
}
