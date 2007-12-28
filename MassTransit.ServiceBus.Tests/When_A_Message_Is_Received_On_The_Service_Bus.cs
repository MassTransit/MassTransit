using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_A_Message_Is_Received_On_The_Service_Bus :
        ServiceBusSetupFixture
    {
        [Test]
        public void An_Event_Handler_Should_Be_Called()
        {
        	bool _received = false;
        	ManualResetEvent _receivedEvent = new ManualResetEvent(false);

			EventHandler<MessageReceivedEventArgs<PingMessage>> handler = delegate
        	{
        		_received = true;
        		_receivedEvent.Set();
        	};
			
        	_serviceBus.Endpoint<PingMessage>().MessageReceived += handler;

            PingMessage pm = new PingMessage();
            _serviceBus.Publish(pm);

        	Assert.That(_receivedEvent.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);

        	Assert.That(_received, Is.True);
        }
    }
}
