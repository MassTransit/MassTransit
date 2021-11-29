namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Metadata;
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


    [TestFixture]
    public class A_faulting_consumer_when_fault_publishing_is_disable :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_a_single_fault_when_retried()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            _consumer.Faults.Select().Count().ShouldBe(0);
        }

        PingConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.PublishFaults = false;

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


    [TestFixture]
    public class A_fault_event :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_the_faulted_message_types()
        {
            await InputQueueSendEndpoint.Send<ActualMessageType>(new { });

            _consumer.Faults.Select().Count().ShouldBe(1);

            IReceivedMessage<Fault<BaseMessageType>> fault = _consumer.Faults.Select().FirstOrDefault();
            Assert.That(fault, Is.Not.Null);

            await TestContext.Out.WriteLineAsync(string.Join(Environment.NewLine, fault.Context.Message.FaultMessageTypes));
            await TestContext.Out.WriteLineAsync(string.Join(Environment.NewLine, fault.Context.SupportedMessageTypes));

            Assert.That(fault.Context.Message.FaultMessageTypes.Contains(MessageUrn.ForTypeString<BaseMessageType>()));
            Assert.That(fault.Context.Message.FaultMessageTypes.Contains(MessageUrn.ForTypeString<ActualMessageType>()));

            Assert.That(fault.Context.SupportedMessageTypes.Contains(MessageUrn.ForTypeString<Fault<BaseMessageType>>()));
        }

        BaseMessageConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new BaseMessageConsumer(TimeSpan.FromSeconds(5), InMemoryTestHarness.InactivityToken);

            _consumer.Configure(configurator);
        }


        class BaseMessageConsumer :
            MultiTestConsumer
        {
            readonly ReceivedMessageList<Fault<BaseMessageType>> _faults;

            public BaseMessageConsumer(TimeSpan timeout, CancellationToken testCompleted)
                : base(timeout, testCompleted)
            {
                Fault<BaseMessageType>();
                _faults = Consume<Fault<BaseMessageType>>();
            }

            public IReceivedMessageList<Fault<BaseMessageType>> Faults => _faults;
        }


        public interface BaseMessageType
        {
        }


        public interface ActualMessageType :
            BaseMessageType
        {
        }
    }


    [TestFixture]
    public class A_fault_event_using_try_get_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_the_faulted_message_types()
        {
            await InputQueueSendEndpoint.Send<ActualMessageType>(new { });

            _consumer.Faults.Select().Count().ShouldBe(1);

            IReceivedMessage<Fault<BaseMessageType>> fault = _consumer.Faults.Select().FirstOrDefault();
            Assert.That(fault, Is.Not.Null);

            await TestContext.Out.WriteLineAsync(string.Join(Environment.NewLine, fault.Context.Message.FaultMessageTypes));
            await TestContext.Out.WriteLineAsync(string.Join(Environment.NewLine, fault.Context.SupportedMessageTypes));

            Assert.That(fault.Context.Message.FaultMessageTypes.Contains(MessageUrn.ForTypeString<BaseMessageType>()));
            Assert.That(fault.Context.Message.FaultMessageTypes.Contains(MessageUrn.ForTypeString<ActualMessageType>()));

            Assert.That(fault.Context.SupportedMessageTypes.Contains(MessageUrn.ForTypeString<Fault<BaseMessageType>>()));
            Assert.That(fault.Context.SupportedMessageTypes.Contains(MessageUrn.ForTypeString<Fault<ActualMessageType>>()));
        }

        BaseMessageConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new BaseMessageConsumer(TimeSpan.FromSeconds(5), InMemoryTestHarness.InactivityToken);

            _consumer.Configure(configurator);

            configurator.Consumer<FaultConsumer>();
        }


        class FaultConsumer :
            IConsumer<BaseMessageType>
        {
            public Task Consume(ConsumeContext<BaseMessageType> context)
            {
                if (context.TryGetMessage<ActualMessageType>(out ConsumeContext<ActualMessageType> actualContext))
                    return actualContext.NotifyFaulted(TimeSpan.Zero, TypeCache<FaultConsumer>.ShortName, new IntentionalTestException());

                throw new InvalidOperationException("This was not expected, but gets the job done");
            }
        }


        class BaseMessageConsumer :
            MultiTestConsumer
        {
            readonly ReceivedMessageList<Fault<BaseMessageType>> _faults;

            public BaseMessageConsumer(TimeSpan timeout, CancellationToken testCompleted)
                : base(timeout, testCompleted)
            {
                _faults = Consume<Fault<BaseMessageType>>();
            }

            public IReceivedMessageList<Fault<BaseMessageType>> Faults => _faults;
        }


        public interface BaseMessageType
        {
        }


        public interface ActualMessageType :
            BaseMessageType
        {
        }
    }
}
