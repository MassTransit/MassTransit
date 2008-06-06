namespace MassTransit.ServiceBus.Tests
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_handler_subscription_is_added :
        Specification
	{
		private ServiceBus _serviceBus;
		private IEndpoint _mockEndpoint;
		private ISubscriptionCache _mockSubscriptionCache;
		private readonly Uri queueUri = new Uri("msmq://localhost/test");
		private Subscription _subscription;

        protected override void Before_each()
        {
			_mockEndpoint = DynamicMock<IEndpoint>();
			_mockSubscriptionCache = DynamicMock<ISubscriptionCache>();
			_subscription = new Subscription(typeof (PingMessage).FullName, queueUri);
			_serviceBus = new ServiceBus(_mockEndpoint, _mockSubscriptionCache);
            
        }

        protected override void After_each()
        {
			_serviceBus = null;
			_mockEndpoint = null;
			_mockSubscriptionCache = null;
            
        }


		[Test]
		public void The_bus_should_add_a_subscription_to_the_subscription_cache()
		{
			using (Record())
			{
				Expect.Call(_mockEndpoint.Uri).Return(queueUri).Repeat.Any();
				_mockSubscriptionCache.Add(_subscription);
			}

			using (Playback())
			{
				_serviceBus.Subscribe<PingMessage>(delegate { });
			}
		}
	}
}