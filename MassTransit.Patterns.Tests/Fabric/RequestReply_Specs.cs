namespace MassTransit.Patterns.Tests.Fabric
{
	using System;
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using ServiceBus;

	[TestFixture]
	public class When_a_request_is_sent
	{
		[Test]
		public void A_response_should_be_properly_dispatched_to_the_originator()
		{
			using (MessagingClient client = new MessagingClient())
			using (MessagingServer server = new MessagingServer())
			{
				client.Attach(server);
				server.Attach(client);

				client.Run();
				
			}
		}

		public class MessagingClient : IProduce<RequestMessage>, Patterns.Fabric.IConsume<ResponseMessage>
		{
			private Patterns.Fabric.IConsume<RequestMessage> _consumer;

			public void Attach(Patterns.Fabric.IConsume<RequestMessage> consumer)
			{
				_consumer = consumer;
			}

			public void Detach(Patterns.Fabric.IConsume<RequestMessage> consumer)
			{
				_consumer = null;
			}

			public void Dispose()
			{
			}

			public void Consume(ResponseMessage message)
			{
				throw new System.NotImplementedException();
			}

			public void Run()
			{
				RequestMessage request = new RequestMessage();

				_consumer.Consume(request);
			}
		}

		public class MessagingServer : Patterns.Fabric.IConsume<RequestMessage>, IProduce<ResponseMessage>
		{
			public void Consume(RequestMessage message)
			{
				throw new System.NotImplementedException();
			}

			public void Dispose()
			{
				throw new System.NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new System.NotImplementedException();
			}

			public void Detach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new System.NotImplementedException();
			}
		}

		public class RequestMessage : IMessage
		{
		}

		public class ResponseMessage : IMessage
		{
		}
		
		public interface IRequestReply<T, V> : 
			Patterns.Fabric.IConsume<T>, 
			IProduce<V> 
			where V  : IMessage
			where T : IMessage
		{

		}

		class RespondsTo<T> where T : IMessage
		{
			public interface With<V> : Patterns.Fabric.IConsume<T>, IProduce<V> where V  : IMessage
			{
			}
		}

		class Requests<T> where T : IMessage
		{
			public interface From<V> : IProduce<V>, Patterns.Fabric.IConsume<T> where V : IMessage
			{
				
			}
		}


		public class AbstractSomething { }


		public class RequestHandler : RespondsTo<RequestMessage>.With<ResponseMessage>
		{
			public void Consume(RequestMessage message)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new NotImplementedException();
			}

			public void Detach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new NotImplementedException();
			}
		}

		public class MyServer : IRequestReply<RequestMessage, ResponseMessage>
		{
			public void Consume(RequestMessage message)
			{
				throw new System.NotImplementedException();
			}

			public void Dispose()
			{
				throw new System.NotImplementedException();
			}

			public void Attach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new System.NotImplementedException();
			}

			public void Detach(Patterns.Fabric.IConsume<ResponseMessage> consumer)
			{
				throw new System.NotImplementedException();
			}
		}
	}


}