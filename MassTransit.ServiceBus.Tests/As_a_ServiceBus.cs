namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_ServiceBus
	{
		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			_mockSubscriptionCache = mocks.DynamicMock<ISubscriptionCache>();
			mockEndpoint = mocks.DynamicMock<IEndpoint>();
			mockSendEndpoint = mocks.DynamicMock<IEndpoint>();

			SetupResult.For(mockEndpoint.Uri).Return(new Uri("msmq://localhost/test"));
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			mockEndpoint = null;
			mockSendEndpoint = null;
			_mockSubscriptionCache = null;
		}

		[Test]
		public void When_creating_a_bus()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			ServiceBus bus = new ServiceBus(mocks.DynamicMock<IEndpoint>(), cache);
			SubscriptionClient client = new SubscriptionClient(bus, cache, null);
		}

		[Test]
		public void When_Publishing_a_message()
		{
			Subscription sub =
				new Subscription("MassTransit.ServiceBus.Tests.PingMessage", new Uri("msmq://localhost/test"));
			List<Subscription> subs = new List<Subscription>();
			subs.Add(sub);

			using (mocks.Record())
			{
				Expect.Call(_mockSubscriptionCache.List("MassTransit.ServiceBus.Tests.PingMessage")).Return(subs);
			}
			using (mocks.Playback())
			{
				ServiceBus bus = new ServiceBus(mockEndpoint, _mockSubscriptionCache);
				bus.Publish(new PingMessage());
			}
		}

		[Test]
		public void When_Sending_2_messages()
		{
			using (mocks.Record())
			{
			}
			using (mocks.Playback())
			{
				mockSendEndpoint.Send(new PingMessage());
			}
		}

		[Test]
		public void When_Sending_a_message()
		{
			using (mocks.Record())
			{
			}
			using (mocks.Playback())
			{
				mockSendEndpoint.Send(new PingMessage());
			}
		}

		private MockRepository mocks;
		private ISubscriptionCache _mockSubscriptionCache;
		private IEndpoint mockEndpoint;
		private IEndpoint mockSendEndpoint;
	}
}