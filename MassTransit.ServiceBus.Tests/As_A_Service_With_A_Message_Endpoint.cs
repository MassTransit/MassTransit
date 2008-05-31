namespace MassTransit.ServiceBus.Tests
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_A_Service_With_A_Message_Endpoint
	{
		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
			_mockEndpoint = _mocks.CreateMock<IEndpoint>();
			_mockSubscriptionCache = _mocks.CreateMock<ISubscriptionCache>();
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
		public void I_Want_To_Be_Able_To_Register_An_Event_Handler_For_Messages()
		{
			using (_mocks.Record())
			{
				Expect.Call(_mockEndpoint.Uri).Return(queueUri).Repeat.Any(); //stupid log4net
				_mockSubscriptionCache.Add(new Subscription(typeof (PingMessage).FullName, queueUri));
				LastCall.IgnoreArguments();
			}

			using (_mocks.Playback())
			{
				_serviceBus = new ServiceBus(_mockEndpoint, _mockSubscriptionCache);
				_serviceBus.Subscribe<PingMessage>(delegate { });
			}
		}

		private ServiceBus _serviceBus;
		private MockRepository _mocks;
		private IEndpoint _mockEndpoint;
		private ISubscriptionCache _mockSubscriptionCache;

		private readonly Uri queueUri = new Uri("msmq://localhost/test");
	}
}