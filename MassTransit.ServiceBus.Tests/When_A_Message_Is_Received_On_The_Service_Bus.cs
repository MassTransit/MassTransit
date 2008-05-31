namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_A_Message_Is_Received_On_The_Service_Bus
	{
		private MockRepository _mocks;
		private ServiceBus _serviceBus;
		private IEndpoint _mockServiceBusEndPoint;
		private bool _received = false;
		private readonly PingMessage _message = new PingMessage();

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

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
			_mockServiceBusEndPoint = _mocks.DynamicMock<IEndpoint>();
			_serviceBus = new ServiceBus(_mockServiceBusEndPoint);
		}

		[TearDown]
		public void TearDown()
		{
			_mocks = null;
			_serviceBus = null;
			_mockServiceBusEndPoint = null;
		}


		[Test]
		public void An_Event_Handler_Should_Be_Called()
		{
			_mocks.ReplayAll();

			Action<IMessageContext<PingMessage>> handler = delegate { _received = true; };
			_serviceBus.Subscribe(handler);

			_serviceBus.Dispatch(_message);

			Assert.That(_received, Is.True);
		}

		[Test]
		public void Dispatching_a_message_asynchronously_should_Dispatch_the_message()
		{
			_mocks.ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			Action<IMessageContext<PingMessage>> handler = delegate { received.Set(); };
			_serviceBus.Subscribe(handler);

			_serviceBus.Dispatch(_message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void Dispatching_a_message_asynchronously_to_an_object_should_Dispatch_the_message()
		{
			_mocks.ReplayAll();

			ManualResetEvent received = new ManualResetEvent(false);

			TestConsumer<PingMessage> consumer = new TestConsumer<PingMessage>(delegate { received.Set(); });
			_serviceBus.Subscribe(consumer);

			_serviceBus.Dispatch(_message, DispatchMode.Asynchronous);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(30), true), Is.True, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void If_there_are_no_subscriptions_the_message_should_be_ignored()
		{
			_mocks.ReplayAll();

			_serviceBus.Dispatch(_message);

			//the test is kind of that no errors happened.
		}
	}
}