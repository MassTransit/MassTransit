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
			string address = "msmq://localhost/mt_client";

			MsmqEndpoint endpoint = new MsmqEndpoint(address);

            Assert.That(endpoint.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\mt_client"));
            Assert.That(endpoint.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));


			MsmqEndpoint endpoint2 = new MsmqEndpoint(new Uri(address));

            Assert.That(endpoint2.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\mt_client"));
            Assert.That(endpoint2.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
		}

		[Test, ExpectedException(typeof (EndpointException))]
		public void An_address_cant_contain_a_path_specifier()
		{
			string address = "msmq://localhost/test_endpoint/error_creator";

			new MsmqEndpoint(address);
		}
	}
}