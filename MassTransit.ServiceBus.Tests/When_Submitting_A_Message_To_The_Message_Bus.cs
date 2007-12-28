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
            IEndpoint endpoint = _serviceBus.DefaultEndpoint;

            Assert.That(endpoint, Is.Not.Null);

            Assert.That(endpoint.Address, Is.EqualTo(_serviceBusEndPoint.Address));
        }
    }
}