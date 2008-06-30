namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class MessageGroup_Specs :
		Specification
	{
		[Test]
		public void I_should_be_able_to_join_a_bunch_of_messages_into_a_group()
		{
			object[] items = new object[] {new PingMessage(), new PongMessage()};


			MessageGroup group = MessageGroup.Join(items);

			Assert.That(group.Count, Is.EqualTo(2));

			Assert.That(group[0], Is.TypeOf(typeof (PingMessage)));
			Assert.That(group[1], Is.TypeOf(typeof (PongMessage)));
		}

		[Test]
		public void I_should_be_able_to_retrieve_a_single_message_by_position()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build()
				.Add(ping)
				.Add(pong);

			PingMessage thePing = group.Get<PingMessage>(0);
		}

		[Test]
		public void I_should_be_able_to_split_a_bunch_of_messages_from_a_group()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build()
				.Add(ping)
				.Add(pong);

			object[] items = group.ToArray();

			Assert.That(items.Length, Is.EqualTo(2));
		}

		[Test]
		public void I_should_be_able_to_split_the_group_into_individual_messages_and_handle_each_one_on_its_own()
		{
			IServiceBus bus = DynamicMock<IServiceBus>();

			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build()
				.Add(ping)
				.Add(pong);

			using (Record())
			{
				bus.Publish<PingMessage>(ping);
				bus.Publish<PongMessage>(pong);
			}

			using (Playback())
			{
				group.Split(bus);
			}
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void I_should_get_an_exception_when_I_try_to_get_an_unmatched_type()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build()
				.Add(ping)
				.Add(pong);

			PingMessage thePing = group.Get<PingMessage>(1);
		}

		[Test]
		public void One()
		{
			IServiceBus bus = DynamicMock<IServiceBus>();

			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build()
				.Add(ping)
				.Add(pong);

			Assert.That(group.Count, Is.EqualTo(2));

			Assert.That(group[0], Is.TypeOf(typeof (PingMessage)));
			Assert.That(group[1], Is.TypeOf(typeof (PongMessage)));

			bus.Publish(group);
		}
	}
}