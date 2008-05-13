namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Messaging;
	using System.Runtime.Serialization.Formatters.Binary;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_sending_directly_to_an_endpoint
	{
		[Test]
		public void The_message_should_be_added_to_the_queue()
		{
			MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();

				CustomMessage message = new CustomMessage();

				endpoint.Send(message);

				Message msg = queue.Receive(TimeSpan.FromSeconds(3));

				object obj = new BinaryFormatter().Deserialize(msg.BodyStream);

				Assert.That(obj, Is.Not.Null);
				if (obj != null)
				{
					Assert.That(obj, Is.TypeOf(typeof (CustomMessage)));
				}
			}
		}
	}

	[TestFixture]
	public class When_receiving_directly_from_an_endpoint
	{
		[Test]
		public void The_message_should_be_read_from_the_queue()
		{
			MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();

				CustomMessage message = new CustomMessage();

				endpoint.Send(message);

				object obj = endpoint.Receive();

				Assert.That(obj, Is.Not.Null);
				if (obj != null)
				{
					Assert.That(obj, Is.TypeOf(typeof (CustomMessage)));
				}
			}
		}

		[Test]
		public void The_message_should_be_read_from_the_queue_using_a_typed_message_handler()
		{
			MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();

				CustomMessage message = new CustomMessage();

				endpoint.Send(message);

				CustomMessage readMessage = endpoint.Receive<CustomMessage>();

				Assert.That(readMessage, Is.Not.Null);
			}
		}

		[Test]
		public void The_message_should_be_read_from_the_queue_using_a_typed_message_handler_skipping_unmatched()
		{
			MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();

				endpoint.Send(new WrongMessage());
				endpoint.Send(new CustomMessage());

				CustomMessage customMessage = endpoint.Receive<CustomMessage>();

				Assert.That(customMessage, Is.Not.Null);

				WrongMessage wrongMessage = endpoint.Receive<WrongMessage>();

				Assert.That(wrongMessage, Is.Not.Null);
			}
		}

		[Test]
		public void The_message_should_be_read_from_the_queue_using_a_typed_message_handler_with_a_filter()
		{
			MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();

				CustomMessage message = new CustomMessage();

				endpoint.Send(message);

				const string name = "Johnson";

				message.Name = name;

				endpoint.Send(message);

				CustomMessage readMessage = endpoint.Receive<CustomMessage>(
					delegate(CustomMessage msg) { return Equals(msg.Name, name); });

				Assert.That(readMessage, Is.Not.Null);

				Assert.That(readMessage.Name, Is.EqualTo(name));
			}
		}
	}

	[Serializable]
	public class CustomMessage
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}

	[Serializable]
	public class WrongMessage
	{
	}
}