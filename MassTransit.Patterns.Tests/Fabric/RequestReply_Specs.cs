namespace MassTransit.Patterns.Tests.Fabric
{
	using System;
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using ServiceBus;

	[TestFixture]
	public class When_a_request_is_sent
	{
		public class MessagingClient : IProduce<RequestMessage>, Patterns.Fabric.IConsume<ResponseMessage>
		{
			private Patterns.Fabric.IConsume<RequestMessage> _consumer;

			#region IConsume<ResponseMessage> Members

			public void Consume(ResponseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IProduce<RequestMessage> Members

			public void Attach(Patterns.Fabric.IConsume<RequestMessage> consumer)
			{
				_consumer = consumer;
			}

			#endregion

			public void Run()
			{
				RequestMessage request = new RequestMessage();

				_consumer.Consume(request);
			}
		}

		public class MessagingServer : RespondsTo<RequestMessage>.With<ResponseMessage>
		{
			#region With<ResponseMessage> Members

			public void Consume(RequestMessage message)
			{
				throw new NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class RequestMessage : IMessage
		{
		}

		public class ResponseMessage : IMessage
		{
		}

		private class Requests<T> where T : IMessage
		{
			#region Nested type: From

			public interface From<V> : IProduce<V>, Patterns.Fabric.IConsume<T> where V : IMessage
			{
			}

			#endregion
		}


		public class AbstractSomething
		{
		}


		public class RequestHandler : RespondsTo<RequestMessage>.With<ResponseMessage>
		{
			#region With<ResponseMessage> Members

			public void Consume(RequestMessage message)
			{
				throw new NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class MyServer : RespondsTo<RequestMessage>.With<ResponseMessage>
		{
			#region With<ResponseMessage> Members

			public void Consume(RequestMessage message)
			{
				throw new NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		[Test]
		public void A_response_should_be_properly_dispatched_to_the_originator()
		{
			MessagingClient client = new MessagingClient();
			MessagingServer server = new MessagingServer();

			client.Attach(server);

			server.Attach(client);


			client.Run();
		}
	}
}