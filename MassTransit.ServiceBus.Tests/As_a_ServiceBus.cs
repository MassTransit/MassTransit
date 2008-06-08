namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_ServiceBus :
        Specification
	{
        protected override void Before_each()
        {
            _mockSubscriptionCache = DynamicMock<ISubscriptionCache>();
            mockEndpoint = DynamicMock<IEndpoint>();
            mockSendEndpoint = DynamicMock<IEndpoint>();

            SetupResult.For(mockEndpoint.Uri).Return(new Uri("msmq://localhost/test"));
        }

	    
		protected override void  After_each()
		{
			mockEndpoint = null;
			mockSendEndpoint = null;
			_mockSubscriptionCache = null;
		}

		[Test]
		public void When_creating_a_bus()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			ServiceBus bus = new ServiceBus(DynamicMock<IEndpoint>(), DynamicMock<IObjectBuilder>(), cache);
			SubscriptionClient client = new SubscriptionClient(bus, cache, null);
		}

		[Test]
		public void When_Publishing_a_message()
		{
			Subscription sub =
				new Subscription("MassTransit.ServiceBus.Tests.PingMessage", new Uri("msmq://localhost/test"));
			List<Subscription> subs = new List<Subscription>();
			subs.Add(sub);

		    IObjectBuilder obj = StrictMock<IObjectBuilder>();
		    IEndpoint dep = DynamicMock<IEndpoint>();
            //TODO: Hacky
			using (Record())
			{
				Expect.Call(_mockSubscriptionCache.List("MassTransit.ServiceBus.Tests.PingMessage")).Return(subs);
			    Expect.Call(obj.Build<IEndpoint>(new Hashtable())).Return(dep).IgnoreArguments();
			}
			using (Playback())
			{
				ServiceBus bus = new ServiceBus(mockEndpoint, obj, _mockSubscriptionCache);
				bus.Publish(new PingMessage());
			}
		}

		[Test]
		public void When_Sending_2_messages()
		{
			using (Record())
			{
			}
			using (Playback())
			{
				mockSendEndpoint.Send(new PingMessage());
			}
		}

		[Test]
		public void When_Sending_a_message()
		{
			using (Record())
			{
			}
			using (Playback())
			{
				mockSendEndpoint.Send(new PingMessage());
			}
		}

		private ISubscriptionCache _mockSubscriptionCache;
		private IEndpoint mockEndpoint;
		private IEndpoint mockSendEndpoint;
	}
}