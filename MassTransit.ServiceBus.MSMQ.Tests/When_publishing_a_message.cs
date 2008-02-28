namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Threading;
	using Internal;
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

				Thread.Sleep(TimeSpan.FromSeconds(3));

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

				Thread.Sleep(TimeSpan.FromSeconds(3));

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

				Thread.Sleep(TimeSpan.FromSeconds(5));

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

				MessageReceivedCallback<UpdateMessage> handler =
					delegate(IMessageContext<UpdateMessage> ctx)
						{
							_updateEvent.Set();

							ctx.Bus.Send(ctx.Envelope.ReturnEndpoint, new UpdateAcceptedMessage());
						};

				ManualResetEvent _repliedEvent = new ManualResetEvent(false);

				qtc.RemoteServiceBus.Subscribe(handler);

				qtc.ServiceBus.Subscribe<UpdateAcceptedMessage>(
					delegate { _repliedEvent.Set(); });

				Thread.Sleep(TimeSpan.FromSeconds(5));

				UpdateMessage um = new UpdateMessage();

				qtc.ServiceBus.Publish(um);

				Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
				            "Timeout expired waiting for message");

				Assert.That(_repliedEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "NO response message received");
			}
		}
	}
}