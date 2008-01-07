using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
	[TestFixture]
	public class When_filtering_messages_by_condition
	{
		[Test]
		public void The_message_should_remain_in_the_queue_if_the_condition_is_not_met()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _called = new ManualResetEvent(false);

				MessageReceivedCallback<ClientMessage> handler =
					delegate(MessageContext<ClientMessage> context) { Assert.That(context.Message.Name, Is.EqualTo("JOHNSON"), "We should not have received this message"); };

				Predicate<ClientMessage> condition =
					delegate(ClientMessage message)
					{
						Assert.That(message.Name, Is.EqualTo("MAGIC"));

						_called.Set();

						if (message.Name != "JOHNSON")
							return false;

						return true;
					};

				qtc.ServiceBus.Subscribe(handler, condition);

				ClientMessage clientMessage = new ClientMessage();
				clientMessage.Name = "MAGIC";

				qtc.ServiceBus.Publish(clientMessage);

				Assert.That(_called.WaitOne(TimeSpan.FromSeconds(3), true), Is.True);

				Thread.Sleep(TimeSpan.FromSeconds(2));
			}
		}

		[Test]
		public void The_Message_Should_Be_Retrieved_If_The_Condition_Is_Met()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				ManualResetEvent _called = new ManualResetEvent(false); 
				ManualResetEvent _received = new ManualResetEvent(false);

				MessageReceivedCallback<ClientMessage> handler =
					delegate(MessageContext<ClientMessage> context)
					{
						Assert.That(context.Message.Name, Is.EqualTo("JOHNSON"), "We should not have received this message");

						_received.Set();
					};

				Predicate<ClientMessage> condition =
					delegate(ClientMessage message)
					{
						Assert.That(message.Name, Is.EqualTo("JOHNSON"));

						_called.Set();

						if (message.Name != "JOHNSON")
							return false;

						return true;
					};

				qtc.ServiceBus.Subscribe(handler, condition);

				ClientMessage clientMessage = new ClientMessage();
				clientMessage.Name = "JOHNSON";

				qtc.ServiceBus.Publish(clientMessage);

				Assert.That(_called.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "Timeout waiting for condition check");

				Assert.That(_received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "Timeout waiting for message");
			}
		}
	}
}