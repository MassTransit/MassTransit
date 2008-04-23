namespace MassTransit.Patterns.Tests.Fabric
{
	using System;
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using ServiceBus;

	[TestFixture]
	public class When_a_request_is_sent
	{
		public class MessagingClient : Produces<RequestMessage>, Patterns.Fabric.Consumes<ResponseMessage>
		{
			private Patterns.Fabric.Consumes<RequestMessage> _consumer;

			#region Consumes<ResponseMessage> Members

			public void Consume(ResponseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Produces<RequestMessage> Members

			public void Attach(Patterns.Fabric.Consumes<RequestMessage> consumer)
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

			public void Attach(Patterns.Fabric.Consumes<ResponseMessage> consumer)
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

			public interface From<V> : Produces<V>, Patterns.Fabric.Consumes<T> where V : IMessage
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

			public void Attach(Patterns.Fabric.Consumes<ResponseMessage> consumer)
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

			public void Attach(Patterns.Fabric.Consumes<ResponseMessage> consumer)
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