namespace MassTransit.Tests.Serialization
{
	using System.Text;
	using Context;
	using NUnit.Framework;
	using Shouldly;


    public class When_a_message_is_send_using_a_transport
	{
		byte[] _bytes;

		[SetUp]
		public void A_message_is_send_using_a_transport()
		{
			var headers = new TransportMessageHeaders();
			headers.Add("Content-Type", "application/vnd.masstransit+json");

			_bytes = headers.GetBytes();
		}

		[Test]
		public void Should_include_the_content_type_as_a_binary_header()
		{
			string value = Encoding.UTF8.GetString(_bytes);

			value.ShouldBe("{\"Content-Type\":\"application/vnd.masstransit+json\"}");
		}

		[Test]
		public void Should_reload_the_byte_array_into_a_headers_object()
		{
			var headers = TransportMessageHeaders.Create(_bytes);

			headers["Content-Type"].ShouldBe("application/vnd.masstransit+json");
		}

		[Test]
		public void Should_return_an_empty_array_for_no_headers()
		{
			var headers = new TransportMessageHeaders();
			headers.Add("Content-Type", null);
			headers.GetBytes().Length.ShouldBe(0);
		}
	}
}
