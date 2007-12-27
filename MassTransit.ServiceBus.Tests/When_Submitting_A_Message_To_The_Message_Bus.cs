using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Submitting_A_Message_To_The_Message_Bus : 
        ServiceBusSetupFixture
    {
		//[Test]
		//public void The_Type_Of_Message_Should_Be_Used_To_Resolve_The_Destination()
		//{
		//    PingMessage message = new PingMessage();

		//    _serviceBus.Send(message);
		//}

        [Test]
        public void A_Destination_May_Be_Specified()
        {
            PingMessage ping = new PingMessage();

			_serviceBus.Send(_testEndPoint, ping);

			VerifyMessageInQueue(_testEndPoint.Transport.Address, ping);
        }

        [Test]
        public void The_Message_Should_Reach_The_Destination_Queue()
        {
            PingMessage ping = new PingMessage();

            _serviceBus.Send(ping);

			VerifyMessageInQueue(_serviceBus.DefaultEndpoint.Transport.Address, ping);
        }


    	[Test]
        public void The_Address_Of_The_Message_Bus_Transport_Should_Be_Available()
        {
            ITransport transport = _serviceBus.DefaultEndpoint.Transport;

            Assert.That(transport, Is.Not.Null);

            Assert.That(transport.Address, Is.EqualTo(_serviceBusEndPoint.Transport.Address));
        }
    }
}