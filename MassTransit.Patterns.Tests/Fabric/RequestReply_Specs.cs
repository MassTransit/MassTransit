namespace MassTransit.Patterns.Tests.Fabric
{
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	[TestFixture]
	public class When_a_request_is_sent
	{
		public class MessagingClient : Produces<RequestMessage>, Consumes<ResponseMessage>
		{
			private Consumes<RequestMessage> _consumer;
			private bool _responseReceived;

			public bool ResponseReceived
			{
				get { return _responseReceived; }
			}

			#region Consumes<ResponseMessage> Members

			public void Consume(ResponseMessage message)
			{
				_responseReceived = true;
			}

			#endregion

			#region Produces<RequestMessage> Members

			public void Attach(Consumes<RequestMessage> consumer)
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
			private Consumes<ResponseMessage> _consumer;
			private bool _requestReceived;

			public bool RequestReceived
			{
				get { return _requestReceived; }
			}

			#region With<ResponseMessage> Members

			public void Consume(RequestMessage message)
			{
				_requestReceived = true;

				if(_consumer != null)
					_consumer.Consume(new ResponseMessage());
			}

			public void Attach(Consumes<ResponseMessage> consumer)
			{
				_consumer = consumer;
			}

			#endregion
		}

		public class RequestMessage : IMessage
		{
		}

		public class ResponseMessage : IMessage
		{
		}

		[Test]
		public void A_response_should_be_properly_dispatched_to_the_originator()
		{
			MessagingClient client = new MessagingClient();
			MessagingServer server = new MessagingServer();

			client.Attach(server);

			server.Attach(client);

			client.Run();

			Assert.That(server.RequestReceived, Is.True, "No Request Received");
			Assert.That(client.ResponseReceived, Is.True, "No Response Received");
		}

		[Test]
		public void A_message_sent_through_a_router_should_arrive()
		{
			MessagingClient client = new MessagingClient();
			MessagingServer server = new MessagingServer();
			MessagingServer server2 = new MessagingServer();
			MessageRouter<RequestMessage> requestRouter = new MessageRouter<RequestMessage>(server, server2);

			client.Attach(requestRouter);

			server.Attach(client);

			client.Run();

			Assert.That(server.RequestReceived, Is.True, "No Request Received");
			Assert.That(server2.RequestReceived, Is.True, "No Request Received");
			Assert.That(client.ResponseReceived, Is.True, "No Response Received");
			
		}
	}
}