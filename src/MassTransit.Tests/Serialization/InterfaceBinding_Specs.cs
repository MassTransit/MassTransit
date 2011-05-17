namespace MassTransit.Tests.Serialization
{
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Context;
	using Magnum.TestFramework;
	using MassTransit.Serialization;

	[Scenario]
	public class When_an_interface_message_is_bound
	{
		string _message;
		JsonMessageSerializer _serializer;

		[When]
		public void An_interface_message_is_bound()
		{
			_serializer = new JsonMessageSerializer();

			var message = new A {TextA = "ValueA"};

			using(var buffer = new MemoryStream())
			{
				_serializer.Serialize<A>(buffer, message.ToSendContext());

				_message = Encoding.UTF8.GetString(buffer.ToArray());

			//	Trace.WriteLine(_message);
			}
		}

		[Then]
		public void Should_receive_the_message_in_the_type_requested()
		{
			using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(_message)))
			{
				IReceiveContext receiveContext = buffer.ToReceiveContext();
				_serializer.Deserialize(receiveContext);

				IConsumeContext<A> context;
				receiveContext.TryGetContext<A>(out context).ShouldBeTrue();

				context.ShouldNotBeNull();
			}
		}

		class A
		{
			public string TextA { get; set; }
		}
	}
}
