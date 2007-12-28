using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Submitting_A_Request_Message :
        ServiceBusSetupFixture
    {
        [Test]
        [Ignore("Not Implemented")]
        public void The_Caller_Should_Be_Able_To_Wait_On_The_Response()
        {
        	_log.Debug("Sending Request Message");

            PingMessage pm = new PingMessage();

            IServiceBusAsyncResult asyncResult = _serviceBus.Request(pm);

        	Assert.That(asyncResult, Is.Not.Null);
        }
    }
}