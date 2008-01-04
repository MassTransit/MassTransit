using System;
using MassTransit.ServiceBus.Tests.Messages;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Submitting_A_Request_Message :
        ServiceBusSetupFixture
    {
        [Test]
        public void The_Caller_Should_Be_Able_To_Wait_On_The_Response()
        {
            _log.Debug("Sending Request Message");

            PingMessage pm = new PingMessage();

            IServiceBusAsyncResult asyncResult = _serviceBus.Request(_serviceBusEndPoint, pm);

            Assert.That(asyncResult, Is.Not.Null);
        }

        [Test]
        public void The_Response_Should_Trigger_The_Calling_Process()
        {
            PingMessage ping = new PingMessage();

            _serviceBus.MessageEndpoint<PingMessage>().Subscribe(
                delegate(MessageContext<PingMessage> context) { context.Reply(new PongMessage()); });


            IServiceBusAsyncResult asyncResult = _serviceBus.Request(_serviceBusEndPoint, ping);

            Assert.That(asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true), Is.True,
                        "Timeout Expired Waiting For Response");

            Assert.That(asyncResult.Messages, Is.Not.Null);

            Assert.That(asyncResult.Messages, Is.Not.Empty);

            PongMessage pong = asyncResult.Messages[0] as PongMessage;

            Assert.That(pong, Is.Not.Null);
        }
    }
}