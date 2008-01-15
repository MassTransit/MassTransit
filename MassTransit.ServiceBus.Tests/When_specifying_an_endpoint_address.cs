using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    using Exceptions;

    [TestFixture]
	public class When_specifying_an_endpoint_address
	{
		[Test]
		public void The_address_should_be_entered_in_a_URI_style_format()
		{
			string address = "msmq://localhost/test_endpoint";

            Uri endpointUri = new Uri(address);

			Assert.That(endpointUri.Scheme, Is.EqualTo("msmq"));

			Assert.That(endpointUri.Host, Is.EqualTo("localhost"));

			Assert.That(endpointUri.AbsolutePath, Is.EqualTo("/test_endpoint"));
		}

		[Test, ExpectedException(typeof(UriFormatException))]
		public void An_address_in_an_invalid_format_should_throw_an_exception()
		{
			string address = @".\private$\bogus";

            new Uri(address);
		}

        [Test]
        public void A_message_queue_address_should_convert_to_a_queue_path()
        {
            string address = "msmq://localhost/test_endpoint";

            IMessageQueueEndpoint endpoint = new MessageQueueEndpoint(address);

            Assert.That(endpoint.QueueName, Is.EqualTo(Environment.MachineName.ToLowerInvariant() + @"\private$\test_endpoint"));
            Assert.That(endpoint.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));


            IMessageQueueEndpoint endpoint2 = new MessageQueueEndpoint(new Uri(address));

            Assert.That(endpoint2.QueueName, Is.EqualTo(Environment.MachineName.ToLowerInvariant() + @"\private$\test_endpoint"));
            Assert.That(endpoint2.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
        }

        [Test, ExpectedException(typeof(EndpointException))]
        public void An_address_cant_contain_a_path_specifier()
        {
            string address = "msmq://localhost/test_endpoint/error_creator";

            new MessageQueueEndpoint(address);
        }
	}
}