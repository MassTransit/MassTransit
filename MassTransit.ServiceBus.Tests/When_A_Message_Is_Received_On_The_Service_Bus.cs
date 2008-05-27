namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Threading;
    using Internal;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_A_Message_Is_Received_On_The_Service_Bus
    {

        private MockRepository mocks;
        private ServiceBus _serviceBus;
        private IEndpoint mockServiceBusEndPoint;
        private IMessageReceiver mockReceiver;
        bool _received = false;


        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
			mockServiceBusEndPoint = mocks.DynamicMock<IEndpoint>();
            mockReceiver = mocks.DynamicMock<IMessageReceiver>();
            _serviceBus = new ServiceBus(mockServiceBusEndPoint);

            SetupResult.For(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")); //stupid log4net
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _serviceBus = null;
            mockServiceBusEndPoint = null;
        }



        [Test]
        public void An_Event_Handler_Should_Be_Called()
        {
            using (mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
                Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
            }
            using (mocks.Playback())
            {

                Action<IMessageContext<PingMessage>> handler = delegate { _received = true; };
                _serviceBus.Subscribe(handler);

                _serviceBus.Deliver(new Envelope(mockServiceBusEndPoint, new PingMessage()));

                Assert.That(_received, Is.True);
            }
        }

        [Test]
        public void If_there_are_no_subscriptions_the_message_should_be_ignored()
        {
            using (mocks.Record())
            {
                
            }
            using (mocks.Playback())
            {
                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
                _serviceBus.Deliver(envelope);

                //the test is kind of that no errors happened.

            }
        }

		[Test]
		public void Dispatching_a_message_asynchronously_should_deliver_the_message()
		{
			SetupResult.For(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
			mocks.ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			Action<IMessageContext<PingMessage>> handler = delegate { received.Set(); };
			_serviceBus.Subscribe(handler);
			
			PingMessage message = new PingMessage();
			_serviceBus.Dispatch(message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void Dispatching_a_message_asynchronously_to_an_object_should_deliver_the_message()
		{
			SetupResult.For(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
			mocks.ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			TestConsumer<PingMessage> consumer = new TestConsumer<PingMessage>(delegate { received.Set(); });
			_serviceBus.Subscribe(consumer);

			PingMessage message = new PingMessage();
			_serviceBus.Dispatch(message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		internal class TestConsumer<T> : Consumes<T>.Any where T : class
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
    }
}