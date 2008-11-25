namespace MassTransit.NMS.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Subscriptions;

	[TestFixture]
	public class When_publishing_a_message_to_the_queue
	{
		[Test]
		public void The_Message_Should_Arrive()
		{
			ServiceBus bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/published_queue"), null);
			bus.SubscriptionCache.Add(new Subscription(typeof (SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue")));
			bus.Publish(new SimpleMessage("dru"));
		}


		[Test]
		public void The_message_should_be_delivered_to_a_local_subscriber()
		{
			using(NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/local_test_queue"))
            using (IServiceBus bus = new ServiceBus(endpoint, null))
			{
				string name = "Johnson";
				ManualResetEvent received = new ManualResetEvent(false);

				bus.Subscribe<SimpleMessage>(delegate(SimpleMessage message)
				                             	{
				                             		Assert.That(message.Name, Is.EqualTo(name));
				                             		received.Set();
				                             	});

				bus.Publish(new SimpleMessage(name));

				Assert.That(received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);
			}
		}



		[Test]
		public void The_Message_Should_Be_Consumed()
		{
            ServiceBus pub_bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/published_queue"), null);
			ServiceBus sub_bus = new ServiceBus(new NmsEndpoint("activemq://localhost:61616/subscribed_queue"), null);
			pub_bus.SubscriptionCache.Add(new Subscription(typeof (SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue")));
			sub_bus.SubscriptionCache.Add(new Subscription(typeof (SimpleMessage).FullName, new Uri("activemq://localhost:61616/subscribed_queue")));

			ManualResetEvent received = new ManualResetEvent(false);

			bool wasCalled = false;
			sub_bus.Subscribe<SimpleMessage>(delegate
			                                 	{
			                                 		wasCalled = true;
			                                 		received.Set();
			                                 	});

			pub_bus.Publish(new SimpleMessage("dru"));

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);
			Assert.IsTrue(wasCalled);
		}


		/*
		 * namespace MassTransit.MSMQ.Tests
{
	using System;
	using System.Threading;
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_publishing_a_message
	{
		[Test]
		public void Multiple_Local_Services_Should_Be_Available()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _updateEvent = new ManualResetEvent(false);

				qtc.ServiceBus.Subscribe<UpdateMessage>(
					delegate { _updateEvent.Set(); });

				ManualResetEvent _deleteEvent = new ManualResetEvent(false);

				qtc.ServiceBus.Subscribe<DeleteMessage>(
					delegate { _deleteEvent.Set(); });

				DeleteMessage dm = new DeleteMessage();

				qtc.ServiceBus.Publish(dm);

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(4), true), Is.True,
				            "Timeout expired waiting for message");

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(4), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Test]
		public void Multiple_messages_should_be_delivered_to_the_appropriate_remote_subscribers()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _updateEvent = new ManualResetEvent(false);

				qtc.RemoteServiceBus.Subscribe<UpdateMessage>(
					delegate { _updateEvent.Set(); });

				ManualResetEvent _deleteEvent = new ManualResetEvent(false);

				qtc.RemoteServiceBus.Subscribe<DeleteMessage>(
					delegate { _deleteEvent.Set(); });

				DeleteMessage dm = new DeleteMessage();

				qtc.ServiceBus.Publish(dm);

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
				            "Timeout expired waiting for message");

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Test]
		public void The_message_should_be_delivered_to_a_local_subscriber()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _updateEvent = new ManualResetEvent(false);

				qtc.ServiceBus.Subscribe<UpdateMessage>(
					delegate { _updateEvent.Set(); });

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Test]
		public void The_message_should_be_delivered_to_a_remote_subscriber()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _updateEvent = new ManualResetEvent(false);

				qtc.RemoteServiceBus.Subscribe<UpdateMessage>(
					delegate { _updateEvent.Set(); });

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
				            "Timeout expired waiting for message");
			}
		}

		[Test]
		public void The_message_should_be_delivered_to_a_remote_subscriber_with_a_reply()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _updateEvent = new ManualResetEvent(false);

				Action<IMessageContext<UpdateMessage>> handler =
					delegate(IMessageContext<UpdateMessage> ctx)
						{
							_updateEvent.Set();

							qtc.RemoteServiceBus.Publish(new UpdateAcceptedMessage());
						};

				ManualResetEvent _repliedEvent = new ManualResetEvent(false);

				qtc.RemoteServiceBus.Subscribe(handler);

				qtc.ServiceBus.Subscribe<UpdateAcceptedMessage>(
					delegate { _repliedEvent.Set(); });

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
				            "Timeout expired waiting for message");

				Assert.That(_repliedEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "NO response message received");
			}
		}
	}
}*/
	}
}