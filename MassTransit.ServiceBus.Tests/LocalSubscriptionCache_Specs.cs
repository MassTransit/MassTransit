namespace MassTransit.ServiceBus.Tests
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_handler_subscription_is_added
	{
		private ServiceBus _serviceBus;
		private MockRepository _mocks;
		private IEndpoint _mockEndpoint;
		private ISubscriptionCache _mockSubscriptionCache;

		private readonly Uri queueUri = new Uri("msmq://localhost/test");
		private Subscription _subscription;

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
			_mockEndpoint = _mocks.DynamicMock<IEndpoint>();
			_mockSubscriptionCache = _mocks.DynamicMock<ISubscriptionCache>();
			_subscription = new Subscription(typeof (PingMessage).FullName, queueUri);
			_serviceBus = new ServiceBus(_mockEndpoint, _mockSubscriptionCache);
		}

		[TearDown]
		public void TearDown()
		{
			_mocks = null;
			_serviceBus = null;
			_mockEndpoint = null;
			_mockSubscriptionCache = null;
		}

		[Test]
		public void The_bus_should_add_a_subscription_to_the_subscription_cache()
		{
			using (_mocks.Record())
			{
				Expect.Call(_mockEndpoint.Uri).Return(queueUri).Repeat.Any();
				_mockSubscriptionCache.Add(_subscription);
			}

			using (_mocks.Playback())
			{
				_serviceBus.Subscribe<PingMessage>(delegate { });
			}
		}
	}
}