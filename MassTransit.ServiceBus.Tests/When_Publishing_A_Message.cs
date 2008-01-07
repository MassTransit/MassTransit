using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests
{
	[TestFixture]
	public class When_Publishing_A_Message :
		ServiceBusSetupFixture
	{
		//[Test]
		//public void Poison_Letters_Should_Be_Moved_To_A_Poison_Queue()
		//{
		//    ManualResetEvent updateEvent = new ManualResetEvent(false);


		//    //this ends up in a seperate thread and I am therefore unable to figure out how to test
		//    _serviceBus.Subscribe<PoisonMessage>(delegate(MessageContext<PoisonMessage> cxt)
		//                                                               {
		//                                                                   try
		//                                                                   {
		//                                                                       cxt.Message.ThrowException();
		//                                                                   }
		//                                                                   catch(Exception)
		//                                                                   {
		//                                                                       cxt.MarkPoison();
		//                                                                   }
		//                                                               }

		//        );

		//   _serviceBus.Publish(new PoisonMessage());

		//    updateEvent.WaitOne(TimeSpan.FromSeconds(3), true);
		//    VerifyMessageInQueue(_serviceBus.Endpoint.PoisonEndpoint.Address, new PoisonMessage());
		//}
	}
}