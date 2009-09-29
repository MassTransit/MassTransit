namespace MassTransit.Transports.Msmq.Tests.Serialization
{
	using Configuration;
	using Magnum.DateTimeExtensions;
	using MassTransit.Serialization;
	using MassTransit.Tests;
	using MassTransit.Tests.Messages;
	using NUnit.Framework;
	using TestFixtures;

	[TestFixture, Explicit]
	public class When_sending_a_message_using_the_specified_serializer<TSerializer> :
		MsmqEndpointTestFixture
		where TSerializer : IMessageSerializer
	{
		protected override void AdditionalEndpointFactoryConfiguration(IEndpointFactoryConfigurator x)
		{
			x.SetDefaultSerializer<TSerializer>();
		}

		[Test]
		public void The_destination_address_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(RemoteBus.Endpoint.Uri, CurrentMessage.Headers.DestinationAddress);

				received.Set(message);
			});

			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_fault_address_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.FaultAddress);

				received.Set(message);
			});

			LocalBus.Publish(ping, context => context.SendFaultTo(LocalBus.Endpoint.Uri));

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_message_type_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(typeof(PingMessage).ToMessageName(), CurrentMessage.Headers.MessageType);

				received.Set(message);
			});

			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_response_address_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.ResponseAddress);

				received.Set(message);
			});

			LocalBus.Publish(ping, context => context.SendResponseTo(LocalBus.Endpoint.Uri));

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_retry_count_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			var retryCount = 69;
			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(retryCount, CurrentMessage.Headers.RetryCount);

				received.Set(message);
			});

			LocalBus.Publish(ping, context => context.SetRetryCount(retryCount));

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}

		[Test]
		public void The_source_address_should_be_properly_set_on_the_message_envelope()
		{
			PingMessage ping = new PingMessage();

			FutureMessage<PingMessage> received = new FutureMessage<PingMessage>();

			RemoteBus.Subscribe<PingMessage>(message =>
			{
				Assert.AreEqual(LocalBus.Endpoint.Uri, CurrentMessage.Headers.SourceAddress);

				received.Set(message);
			});

			LocalBus.Publish(ping);

			Assert.IsTrue(received.IsAvailable(3.Seconds()), "Timeout waiting for message");
		}
	}

	[TestFixture, Category("Integration")]
	public class For_the_binary_message_serializer :
		When_sending_a_message_using_the_specified_serializer<BinaryMessageSerializer>
	{
	}

	[TestFixture, Category("Integration")]
	public class For_the_XML_message_serializer :
		When_sending_a_message_using_the_specified_serializer<DotNotXmlMessageSerializer>
	{
	}

	[TestFixture, Category("Integration")]
	public class For_the_custom_xml_message_serializer :
		When_sending_a_message_using_the_specified_serializer<XmlMessageSerializer>
	{
	}
}