using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{

    [TestFixture]
    public class WriteOnlyMessageQueueEndpointTests :
        ServiceBusSetupFixture
    {
        private MessageQueueEndpoint endpoint;

        public override void Before_Each_Test_In_The_Fixture()
        {
            base.Before_Each_Test_In_The_Fixture();
            endpoint = new MessageQueueEndpoint(_poisonQueueName);
        }

        public override void After_Each_Test_In_The_Fixture()
        {
            base.After_Each_Test_In_The_Fixture();
            endpoint = null;
        }

        [Test]
        public void Address_Is_Right()
        {
            Assert.That(endpoint.Uri.AbsoluteUri, Is.EqualTo("msmq://localhost/test_servicebus_poison"));
        }

        [Test]
        public void Writes_Correctly()
        {
            DeleteMessage msg = new DeleteMessage();
            Envelope env = new Envelope(base._testEndPoint, msg);
            MessageSenderFactory.Create(endpoint).Send(env);
            VerifyMessageInQueue(endpoint.QueueName, msg);
        }
    }
}