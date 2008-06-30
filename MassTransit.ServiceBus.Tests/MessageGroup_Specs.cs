namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class MessageGroup_Specs : 
		Specification
	{
		
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

	[Serializable]
	public class MessageGroup
	{
		private readonly List<object> _messages;

		public MessageGroup(List<object> messages)
		{
			_messages = messages;
		}

		public static MessageGroupBuilder Build()
		{
			return new MessageGroupBuilder();
			
		}

		public int Count
		{
			get { return _messages.Count; }
		}

		public object this[int index]
		{
			get { return _messages[index]; }
		}
	}

	public class MessageGroupBuilder
	{
		internal readonly List<object> _messages = new List<object>();

		public MessageGroupBuilder Add<T>(T message) where T : class
		{
			_messages.Add(message);

			return this;
		}

		public static implicit operator MessageGroup(MessageGroupBuilder builder)
		{
			return new MessageGroup(builder._messages);
		}
	}
}