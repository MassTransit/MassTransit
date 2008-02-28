namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_specifying_an_endpoint_address
	{
		[Test, ExpectedException(typeof (UriFormatException))]
		public void An_address_in_an_invalid_format_should_throw_an_exception()
		{
			string address = @".\private$\bogus";

			new Uri(address);
		}

		[Test]
		public void The_address_should_be_entered_in_a_URI_style_format()
		{
			string address = "local://localhost/test_endpoint";

			Uri endpointUri = new Uri(address);

			Assert.That(endpointUri.Scheme, Is.EqualTo("local"));

			Assert.That(endpointUri.Host, Is.EqualTo("localhost"));

			Assert.That(endpointUri.AbsolutePath, Is.EqualTo("/test_endpoint"));
		}
	}
}