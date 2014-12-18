namespace MassTransit.Tests.Serialization
{
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Context;
	using NUnit.Framework;
	using MassTransit.Serialization;
	using Shouldly;


    public class When_an_interface_message_is_bound
	{
		string _message;
		JsonMessageSerializer _serializer;

		[SetUp]
		public void An_interface_message_is_bound()
		{
			_serializer = new JsonMessageSerializer();

			var message = new A {TextA = "ValueA"};

			using(var buffer = new MemoryStream())
			{
//				_serializer.Serialize<A>(buffer, message.ToSendContext());

				_message = Encoding.UTF8.GetString(buffer.ToArray());

			//	Trace.WriteLine(_message);
			}
		}

		[Test]
		public void Should_receive_the_message_in_the_type_requested()
		{
//			using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(_message)))
//			{
//				IReceiveContext receiveContext = OldReceiveContext.FromBodyStream(buffer);
//				_serializer.Deserialize(receiveContext);
//
//				IConsumeContext<A> context;
//				receiveContext.TryGetContext<A>(out context).ShouldBe(true);
//
//				context.ShouldNotBe(null);
//			}
		}

		class A
		{
			public string TextA { get; set; }
		}
	}
}
