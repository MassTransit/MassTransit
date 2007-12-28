using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
	[TestFixture]
	public class As_A_Service_With_A_Message_Endpoint :
		ServiceBusSetupFixture
	{
		[Test]
		public void I_Want_To_Be_Able_To_Register_An_Event_Handler_For_Messages()
		{
			_serviceBus.Subscribe<MyUpdateMessage>().MessageReceived += MyUpdateMessage_Received;

			Assert.That(_serviceBus.Subscribe<MyUpdateMessage>(), Is.Not.Null);
		}

		private static void MyUpdateMessage_Received(object sender, MessageReceivedEventArgs<MyUpdateMessage> e)
		{
		}

		[Serializable]
		public class MyUpdateMessage : IMessage
		{
		}
	}
}