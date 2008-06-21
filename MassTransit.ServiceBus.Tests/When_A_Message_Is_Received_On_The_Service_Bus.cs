namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

    [TestFixture]
	public class When_A_Message_Is_Received_On_The_Service_Bus :
        Specification
	{
	    private IObjectBuilder _builder;
		private ServiceBus _serviceBus;
		private IEndpoint _mockServiceBusEndPoint;
		private bool _received = false;
		private readonly PingMessage _message = new PingMessage();
        private readonly Uri _busEndpointUri = new Uri("msmq://localhost/test");

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

        protected override void Before_each()
        {
            _mockServiceBusEndPoint = DynamicMock<IEndpoint>();
            SetupResult.For(_mockServiceBusEndPoint.Uri).Return(_busEndpointUri);
            _builder = DynamicMock<IObjectBuilder>();
            _serviceBus = new ServiceBus(_mockServiceBusEndPoint, _builder);
        }

        protected override void After_each()
        {
            _serviceBus = null;
            _mockServiceBusEndPoint = null;
        }


		[Test]
		public void An_Event_Handler_Should_Be_Called()
		{
			ReplayAll();

			Action<IMessageContext<PingMessage>> handler = delegate { _received = true; };
			_serviceBus.Subscribe(handler);

			_serviceBus.Dispatch(_message);

			Assert.That(_received, Is.True);
		}

		[Test]
		public void Dispatching_a_message_asynchronously_should_Dispatch_the_message()
		{
			ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			Action<IMessageContext<PingMessage>> handler = delegate { received.Set(); };
			_serviceBus.Subscribe(handler);

			_serviceBus.Dispatch(_message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void Dispatching_a_message_asynchronously_to_an_object_should_Dispatch_the_message()
		{
			ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			TestConsumer<PingMessage> consumer = new TestConsumer<PingMessage>(delegate { received.Set(); });
			_serviceBus.Subscribe(consumer);

			_serviceBus.Dispatch(_message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void If_there_are_no_subscriptions_the_message_should_be_ignored()
		{
			ReplayAll();

			_serviceBus.Dispatch(_message);

			//the test is kind of that no errors happened.
		}
	}
}