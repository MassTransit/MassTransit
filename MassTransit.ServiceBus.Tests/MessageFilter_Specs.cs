namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_message_filter_is_subscribed_to_the_service_bus
	{
		private MockRepository _mocks;
		private ServiceBus _serviceBus;
		private IEndpoint _mockServiceBusEndPoint;
		private readonly RequestMessage _message = new RequestMessage();
		private readonly ManualResetEvent _passed = new ManualResetEvent(false);
		private TestConsumer<RequestMessage> _consumer;

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

		public bool FilterFunction(RequestMessage message)
		{
			return false;
		}

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
			_mockServiceBusEndPoint = _mocks.DynamicMock<IEndpoint>();
			_serviceBus = new ServiceBus(_mockServiceBusEndPoint);

			_consumer = new TestConsumer<RequestMessage>(delegate { _passed.Set(); });
		}

		[TearDown]
		public void TearDown()
		{
			_mocks = null;
			_serviceBus = null;
			_mockServiceBusEndPoint = null;
		}

		[Test]
		public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward()
		{
			_mocks.ReplayAll();

			MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return false; }, _consumer);

			_serviceBus.Subscribe(filter);

			_serviceBus.Dispatch(_message);

			Assert.That(_passed.WaitOne(TimeSpan.FromSeconds(0), true), Is.False, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward_success()
		{
			_mocks.ReplayAll();

			MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return true; }, _consumer);

			_serviceBus.Subscribe(filter);

			_serviceBus.Dispatch(_message);

			Assert.That(_passed.WaitOne(TimeSpan.FromSeconds(0), true), Is.True, "Timeout waiting for message handler to be called");
		}
	}
}