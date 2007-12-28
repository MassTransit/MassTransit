using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Submitting_A_Message_To_The_Message_Bus : 
        ServiceBusSetupFixture
    {
    	[Test]
        public void The_Address_Of_The_Message_Bus_Transport_Should_Be_Available()
        {
            ITransport transport = _serviceBus.DefaultEndpoint.Transport;

            Assert.That(transport, Is.Not.Null);

            Assert.That(transport.Address, Is.EqualTo(_serviceBusEndPoint.Transport.Address));
        }
    }
}