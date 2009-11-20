namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using MassTransit.Internal;
    using Messages;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class When_a_message_filter_is_subscribed_to_the_service_bus :
        LoopbackLocalAndRemoteTestFixture
    {
        private readonly RequestMessage _message = new RequestMessage();
        private readonly ManualResetEvent _passed = new ManualResetEvent(false);
        private TestConsumer<RequestMessage> _consumer;

        internal class TestConsumer<T> : Consumes<T>.All where T : class
        {
            private readonly Action<T> callback;

            public TestConsumer(Action<T> callback)
            {
                this.callback = callback;
            }

            public void Consume(T message)
            {
                callback(message);
            }
        }

        public bool FilterFunction(RequestMessage message)
        {
            return false;
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _consumer = new TestConsumer<RequestMessage>(delegate { _passed.Set(); });
        }

        [Test]
        public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward()
        {
            MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return false; }, _consumer);

            LocalBus.Subscribe(filter);
            RemoteBus.Publish(_message);

            Assert.That(_passed.WaitOne(TimeSpan.FromSeconds(1), true), Is.False, "Timeout waiting for message handler to be called");
        }

        [Test]
        public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward_success()
        {
            MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return true; }, _consumer);

            LocalBus.Subscribe(filter);

            RemoteBus.Publish(_message);

            Assert.That(_passed.WaitOne(TimeSpan.FromSeconds(1), true), Is.True, "Timeout waiting for message handler to be called");
        }
    }
}