using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_A_Message_Is_Received_On_The_Service_Bus :
        ServiceBusSetupFixture
    {
        //[Test]
        //public void The_Message_Should_Be_Matched_To_The_Appropriate_Message_Handler()
        //{
        //    IMessageEndpoint<PingMessage> pingEndpoint = new PingMessageHandler(_serviceBus);

        //    _serviceBus.MessageHandlers.Add(pingEndpoint);

        //    PingMessage pm = new PingMessage();

        //    IList<Type> handlerTypes = _serviceBus.MessageHandlers.Find(pm);

        //    Assert.That(handlerTypes, Is.Not.Null);

        //    Assert.That(handlerTypes.Count, Is.EqualTo(1));

        //    Assert.That(handlerTypes[0], Is.EqualTo(pingEndpoint.GetType()));
        //}

        [Test]
        public void An_Event_Handler_Should_Be_Registered_On_The_Message_Endpoint()
        {
            _serviceBus.Endpoint<PingMessage>().MessageReceived += PingReceived;

            PingMessage pm = new PingMessage();

            _serviceBus.Send(pm);

        }

        private void PingReceived(object sender, MessageReceivedEventArgs<PingMessage> e)
        {
            
        }
    }
}
