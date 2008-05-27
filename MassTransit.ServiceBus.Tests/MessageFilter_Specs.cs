namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_message_filter_is_subscribed_to_the_service_bus
	{
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
		public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward()
		{
			SetupResult.For(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
			mocks.ReplayAll();

			ManualResetEvent passed = new ManualResetEvent(false);

			TestConsumer<RequestMessage> consumer = new TestConsumer<RequestMessage>(delegate { passed.Set(); });

			MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return false; }, consumer);

			_serviceBus.Subscribe(filter);

			RequestMessage message = new RequestMessage();
			_serviceBus.Dispatch(message, DispatchMode.Synchronous);

			Assert.That(passed.WaitOne(TimeSpan.FromSeconds(0), true), Is.False, "Timeout waiting for message handler to be called");
		}

		[Test]
		public void A_message_should_only_reach_the_consumer_if_the_filter_passes_it_forward_success()
		{
			SetupResult.For(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
			mocks.ReplayAll();

			ManualResetEvent passed = new ManualResetEvent(false);

			TestConsumer<RequestMessage> consumer = new TestConsumer<RequestMessage>(delegate { passed.Set(); });

			MessageFilter<RequestMessage> filter = new MessageFilter<RequestMessage>(delegate { return true; }, consumer);

			_serviceBus.Subscribe(filter);

			RequestMessage message = new RequestMessage();
			_serviceBus.Dispatch(message, DispatchMode.Synchronous);

			Assert.That(passed.WaitOne(TimeSpan.FromSeconds(0), true), Is.True, "Timeout waiting for message handler to be called");
		}


		private MockRepository mocks;
		private ServiceBus _serviceBus;
		private IEndpoint mockServiceBusEndPoint;
		private IMessageReceiver mockReceiver;

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
	}
}