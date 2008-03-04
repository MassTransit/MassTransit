using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using Internal;

    [TestFixture]
    public class When_a_handler_unsubscribes_from_the_service_bus
    {
        private MockRepository _mocks = new MockRepository();
        private IEndpoint _endpoint;
        private ISubscriptionStorage _storage;
        private IServiceBus _bus;
        private IEnvelopeConsumer _consumer;
        private IEnvelope _envelope = new Envelope(new PingMessage());
        private Uri _endpointUri = new Uri("msmq://localhost/test");
        private IMessageReceiver _receiver;

        [SetUp]
        public void Setup()
        {
            _endpoint = _mocks.CreateMock<IEndpoint>();
            _storage = _mocks.CreateMock<ISubscriptionStorage>();


            _bus = new ServiceBus(_endpoint, _storage);
            _receiver = _mocks.CreateMock<IMessageReceiver>();
        }

        [TearDown]
        public void Teardown()
        {
            
        }

        [Test]
        public void The_service_bus_should_no_longer_show_the_message_type_as_handled()
        {
            using(_mocks.Record())
            {
                Expect.Call(_endpoint.Uri).Return(_endpointUri).Repeat.Any();
                Expect.Call(_endpoint.Receiver).Return(_receiver);
                Expect.Call(delegate { _receiver.Subscribe(_consumer); }).IgnoreArguments();
                Expect.Call(delegate { _storage.Add(null); }).IgnoreArguments();

                Expect.Call(delegate { _storage.Remove(null); }).IgnoreArguments();
            }

            using (_mocks.Playback())
            {
                _consumer = _bus as IEnvelopeConsumer;

                _bus.Subscribe<PingMessage>(HandleAllMessages);
                Assert.That(_consumer.IsHandled(_envelope), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages);
                Assert.That(_consumer.IsHandled(_envelope), Is.False);
            }
        }

        [Test]
        public void The_service_bus_should_continue_to_handle_messages_if_at_least_one_handler_is_available()
        {
            using (_mocks.Record())
            {
                Expect.Call(_endpoint.Uri).Return(_endpointUri).Repeat.Any();
                Expect.Call(delegate { _storage.Add(null); }).IgnoreArguments();
                Expect.Call(_endpoint.Receiver).Return(_receiver);
                Expect.Call(delegate { _receiver.Subscribe(_consumer); }).IgnoreArguments();

                Expect.Call(_endpoint.Receiver).Return(_receiver);
                Expect.Call(delegate { _receiver.Subscribe(_consumer); }).IgnoreArguments();

                Expect.Call(delegate { _storage.Remove(null); }).IgnoreArguments();
            }

            using (_mocks.Playback())
            {
                _consumer = _bus as IEnvelopeConsumer;
                _bus.Subscribe<PingMessage>(HandleAllMessages);
                Assert.That(_consumer.IsHandled(_envelope), Is.True);

                _bus.Subscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
                Assert.That(_consumer.IsHandled(_envelope), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages);
                Assert.That(_consumer.IsHandled(_envelope), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
                Assert.That(_consumer.IsHandled(_envelope), Is.False);
            }
        }


        private static void HandleAllMessages(IMessageContext<PingMessage> ctx)
        {
            
        }

        private static bool HandleSomeMessagesPredicate(PingMessage message)
        {
            return true;
        }
    }
}