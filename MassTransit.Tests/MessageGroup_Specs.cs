namespace MassTransit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Configuration;
	using MassTransit.Internal;
	using MassTransit.Serialization;
	using MassTransit.Transports;
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Subscriptions;
	using TextFixtures;

	[TestFixture]
	public class MessageGroup_Specs :
		LoopbackTestFixture
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

			MessageGroup group = MessageGroup.Build<MessageGroup>()
				.Add(ping)
				.Add(pong);

			PingMessage thePing = group.Get<PingMessage>(0);
		}

		[Test]
		public void I_should_be_able_to_split_a_bunch_of_messages_from_a_group()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build<MessageGroup>()
				.Add(ping)
				.Add(pong);

			object[] items = group.ToArray();

			Assert.That(items.Length, Is.EqualTo(2));
		}

		[Test]
		public void I_should_be_able_to_split_the_group_into_individual_messages_and_handle_each_one_on_its_own()
		{
			IServiceBus bus = MockRepository.GenerateMock<IServiceBus>();

			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build<MessageGroup>()
				.Add(ping)
				.Add(pong);

			bus.Expect(x => x.Publish(ping));
			bus.Expect(x => x.Publish(pong));

			group.Split(bus);

			bus.VerifyAllExpectations();
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void I_should_get_an_exception_when_I_try_to_get_an_unmatched_type()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build<MessageGroup>()
				.Add(ping)
				.Add(pong);

			PingMessage thePing = group.Get<PingMessage>(1);
		}

		[Test]
		public void One()
		{
			PingMessage ping = new PingMessage();
			PongMessage pong = new PongMessage();

			MessageGroup group = MessageGroup.Build<MessageGroup>()
				.Add(ping)
				.Add(pong);

			Assert.That(group.Count, Is.EqualTo(2));

			Assert.That(group[0], Is.TypeOf(typeof (PingMessage)));
			Assert.That(group[1], Is.TypeOf(typeof (PongMessage)));
		}
	}

	[TestFixture]
	public class When_a_custom_message_group_is_defined : 
		LoopbackTestFixture
	{
		internal class Consumer : 
			Consumes<SpecialGroup>.All, 
			Consumes<PingMessage>.All, 
			Consumes<PongMessage>.All
		{
			private readonly IServiceBus _bus;

			private readonly ManualResetEvent _gotPing = new ManualResetEvent(false);
			private readonly ManualResetEvent _gotPong = new ManualResetEvent(false);
			private readonly ManualResetEvent _received = new ManualResetEvent(false);

			public Consumer(IServiceBus bus)
			{
				_bus = bus;
			}

			public ManualResetEvent Received
			{
				get { return _received; }
			}

			public ManualResetEvent GotPing
			{
				get { return _gotPing; }
			}

			public ManualResetEvent GotPong
			{
				get { return _gotPong; }
			}

			#region All Members

			public void Consume(PingMessage message)
			{
				_gotPing.Set();
			}

			#endregion

			#region All Members

			public void Consume(PongMessage message)
			{
				_gotPong.Set();
			}

			#endregion

			#region All Members

			public void Consume(SpecialGroup message)
			{
				_received.Set();

				if (message.SplitOnConsume)
					message.Split(_bus);
			}

			#endregion
		}

		[Serializable]
		[AllowMessageType(typeof (PingMessage), typeof (PongMessage))]
		public class SpecialGroup : MessageGroup
		{
			private bool _splitOnConsume;

			protected SpecialGroup()
			{
			}

			public SpecialGroup(List<object> messages) :
				base(messages)
			{
			}

			public bool SplitOnConsume
			{
				get { return _splitOnConsume; }
				set { _splitOnConsume = value; }
			}
		}

		[Test]
		public void I_should_be_able_to_split_a_group_of_messages_into_parts()
		{
			Consumer c = new Consumer(LocalBus);
			LocalBus.Subscribe(c);

			SpecialGroup group = MessageGroup.Build<SpecialGroup>()
				.Add(new PingMessage())
				.Add(new PongMessage());

			group.SplitOnConsume = true;

			LocalBus.Publish(group);

			Assert.That(c.Received.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No message received by consumer");
			Assert.That(c.GotPing.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No ping received by consumer");
			Assert.That(c.GotPong.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No pong received by consumer");
		}

		[Test]
		public void I_Should_be_able_to_subscribe()
		{
			Consumer c = new Consumer(LocalBus);
			LocalBus.Subscribe(c);

			SpecialGroup group = MessageGroup.Build<SpecialGroup>()
				.Add(new PingMessage())
				.Add(new PongMessage());

			LocalBus.Publish(group);

			Assert.That(c.Received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "No message received by consumer");
		}

		[Test, ExpectedException(typeof (ArgumentException))]
		public void I_should_only_be_allowed_to_add_valid_message_types()
		{
			SpecialGroup group = MessageGroup.Build<SpecialGroup>()
				.Add(new PingMessage())
				.Add(new PongMessage())
				.Add(new UpdateMessage());
		}
	}
}