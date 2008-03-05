namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;
	using Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_ServiceBus
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			_mockSubscriptionCache = mocks.CreateMock<ISubscriptionCache>();
			mockEndpoint = mocks.CreateMock<IEndpoint>();
			mockSendEndpoint = mocks.CreateMock<IEndpoint>();
			mockSender = mocks.CreateMock<IMessageSender>();

			SetupResult.For(mockEndpoint.Uri).Return(new Uri("msmq://localhost/test"));
		}

		[TearDown]
		public void TearDown()
		{
			mocks = null;
			mockEndpoint = null;
			mockSendEndpoint = null;
			_mockSubscriptionCache = null;
			mockSender = null;
		}

		#endregion

		private MockRepository mocks;
		private ISubscriptionCache _mockSubscriptionCache;
		private IEndpoint mockEndpoint;
		private IEndpoint mockSendEndpoint;
		private IMessageSender mockSender;

		[Test, Ignore("endpoint resolver, need to look at - dru")]
		public void When_Publishing_a_message()
		{
			Subscription sub =
				new Subscription("MassTransit.ServiceBus.Tests.PingMessage", new Uri("msmq://localhost/test"));
			List<Subscription> subs = new List<Subscription>();
			subs.Add(sub);

			using (mocks.Record())
			{
				Expect.Call(mockSendEndpoint.Uri).Return(new Uri("msmq://localhost/send"));
				Expect.Call(mockSendEndpoint.Uri).Return(new Uri("msmq://localhost/send"));

				Expect.Call(_mockSubscriptionCache.List("MassTransit.ServiceBus.Tests.PingMessage")).Return(subs);

				//Expect.Call(mockSendEndpoint.Sender).Return(mockSender);
				//mockSender.Send(null);
				//LastCall.IgnoreArguments();
			}
			using (mocks.Playback())
			{
				ServiceBus bus = new ServiceBus(mockEndpoint);
				bus.SubscriptionCache = _mockSubscriptionCache;
				bus.Publish(new PingMessage());
			}
		}

		[Test]
		public void When_Sending_2_messages()
		{
			using (mocks.Record())
			{
				Expect.Call(mockSendEndpoint.Sender).Return(mockSender).Repeat.Twice();
				Expect.Call(delegate { mockSender.Send(null); }).IgnoreArguments().Repeat.Twice();
			}
			using (mocks.Playback())
			{
				ServiceBus bus = new ServiceBus(mockEndpoint);
				bus.SubscriptionCache = _mockSubscriptionCache;
				bus.Send(mockSendEndpoint, new PingMessage(), new PingMessage());
			}
		}

		[Test]
		public void When_Sending_a_message()
		{
			using (mocks.Record())
			{
				Expect.Call(mockSendEndpoint.Sender).Return(mockSender);
				mockSender.Send(null);
				LastCall.IgnoreArguments();
			}
			using (mocks.Playback())
			{
				ServiceBus bus = new ServiceBus(mockEndpoint);
				bus.SubscriptionCache = _mockSubscriptionCache;
				bus.Send(mockSendEndpoint, new PingMessage());
			}
		}
	}
}