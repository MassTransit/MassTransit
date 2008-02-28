namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using Exceptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_specifying_a_message_queue_address_for_an_endpoint
	{
		[Test]
		public void A_message_queue_address_should_convert_to_a_queue_path()
		{
			string address = "msmq://localhost/test_endpoint";

			IMessageQueueEndpoint endpoint = new MessageQueueEndpoint(address);

			Assert.That(endpoint.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\test_endpoint"));
			Assert.That(endpoint.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));


			IMessageQueueEndpoint endpoint2 = new MessageQueueEndpoint(new Uri(address));

			Assert.That(endpoint2.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\test_endpoint"));
			Assert.That(endpoint2.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
		}

		[Test, ExpectedException(typeof (EndpointException))]
		public void An_address_cant_contain_a_path_specifier()
		{
			string address = "msmq://localhost/test_endpoint/error_creator";

			new MessageQueueEndpoint(address);
		}
	}
}