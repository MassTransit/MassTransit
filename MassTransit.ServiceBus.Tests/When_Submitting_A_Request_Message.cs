using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Submitting_A_Request_Message :
        ServiceBusSetupFixture
    {
        [Test]
        public void The_Caller_Should_Be_Able_To_Wait_On_The_Response()
        {
            PingMessage pm = new PingMessage();

            _serviceBus.Request(pm);
        }
    }
}