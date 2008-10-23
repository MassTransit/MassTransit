namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Messaging;
	using Exceptions;
	using NUnit.Framework;

	[TestFixture]
	public class When_working_with_an_endpoint
	{
		[Test, ExpectedException(typeof(EndpointException))]
		public void An_exception_should_be_thrown_for_a_non_existant_queue()
		{
			MsmqEndpoint q = new MsmqEndpoint(new Uri("msmq://localhost/this_queue_does_not_exist"));

			q.Open(QueueAccessMode.ReceiveAndAdmin).GetAllMessages();
		}
	}
}