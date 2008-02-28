using System;
using System.Threading;
using MassTransit.ServiceBus.Internal;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.NMS.Tests
{
	[TestFixture]
	public class When_sending_a_message_to_the_queue
	{
		[Test]
		public void The_message_should_arrive()
		{
			NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name");

			IMessageSender sender = endpoint.Sender;

			SimpleMessage msg = new SimpleMessage();
			msg.Name = "Chris";

			Envelope e = new Envelope(msg);

			sender.Send(e);
		}

		[Test]
		public void The_message_should_be_retrieved()
		{
			NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name");

			IMessageReceiver receiver = endpoint.Receiver;

			ManualResetEvent received = new ManualResetEvent(false);

			EnvelopeConsumer consumer = new EnvelopeConsumer(
				delegate(IEnvelope e)
			{
				received.Set();
			});

			receiver.Subscribe(consumer);

			IMessageSender sender = endpoint.Sender;

			SimpleMessage msg = new SimpleMessage();
			msg.Name = "Chris";

			Envelope env = new Envelope(msg);

			sender.Send(env);

			Assert.That(received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);

			endpoint.Dispose();
			
		}
	}

	public  delegate void EnvelopeHandler(IEnvelope e);

	public class EnvelopeConsumer : 
		IEnvelopeConsumer
	{
		private readonly EnvelopeHandler _eh;

		public EnvelopeConsumer(EnvelopeHandler eh)
		{
			_eh = eh;
		}

		public bool IsHandled(IEnvelope envelope)
		{
			return true;
		}

		public void Deliver(IEnvelope envelope)
		{
			_eh(envelope);
		}
	}

	[Serializable]
	public class SimpleMessage : IMessage
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

	}
}