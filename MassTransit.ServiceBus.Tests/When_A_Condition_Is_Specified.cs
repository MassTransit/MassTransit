using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_A_Condition_Is_Specified :
        ServiceBusSetupFixture
    {
        [Test]
        public void The_Message_Should_Remain_In_The_Queue_If_The_Condition_Is_Not_Met()
        {
            MessageReceivedCallback<ClientMessage> handler =
                delegate(MessageContext<ClientMessage> context)
                    {
                        Assert.That(context.Message.Name, Is.EqualTo("JOHNSON"), "We should not have received this message");
                    };

            Predicate<ClientMessage> condition =
                delegate(ClientMessage message)
                    {
                        if(message.Name != "JOHNSON")
                            return false;

                        return true;
                    };

            _serviceBus.Subscribe(handler, condition);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            ClientMessage clientMessage = new ClientMessage();
            clientMessage.Name = "MAGIC";

            _serviceBus.Publish(clientMessage);

            Thread.Sleep(TimeSpan.FromSeconds(15));
        }

        [Test]
        public void The_Message_Should_Be_Retrieved_If_The_Condition_Is_Met()
        {
            ManualResetEvent _received = new ManualResetEvent(false);

            MessageReceivedCallback<ClientMessage> handler =
                delegate(MessageContext<ClientMessage> context)
                {
                    Assert.That(context.Message.Name, Is.EqualTo("JOHNSON"), "We should not have received this message");

                    _received.Set();
                };

            Predicate<ClientMessage> condition =
                delegate(ClientMessage message)
                {
                    if (message.Name != "JOHNSON")
                        return false;

                    return true;
                };

            _serviceBus.Subscribe(handler, condition);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            ClientMessage clientMessage = new ClientMessage();
            clientMessage.Name = "JOHNSON";

            _serviceBus.Publish(clientMessage);

            Assert.That(_received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "Timeout waiting for message");
        }
    }

    [Serializable]
    public class ClientMessage : IMessage
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}