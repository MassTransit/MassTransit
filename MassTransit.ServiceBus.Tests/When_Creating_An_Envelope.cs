using System;
using MassTransit.ServiceBus.Util;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Creating_An_Envelope
    {
        [Test]
        public void A_Return_Address_Should_Be_Stored_With_The_Envelope()
        {
            MessageQueueEndpoint returnTo = @"msmq://localhost/test_endpoint";

            IEnvelope e = new Envelope(returnTo);

			Assert.That(e.ReturnEndpoint.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
        }

        [Test]
        public void A_Message_Should_Be_Stored_In_The_Envelope()
        {
            MockRepository mocks = new MockRepository();

            IMessage message = mocks.CreateMock<IMessage>();

            MessageQueueEndpoint returnTo = @"msmq://localhost/test_endpoint";

            IEnvelope e = new Envelope(returnTo, message);

            Assert.That(e.Messages.Length, Is.EqualTo(1));
        }

        [Test]
        public void An_Array_Of_Messages_Should_Be_Stored_In_The_Envelope()
        {
            MockRepository mocks = new MockRepository();

            IMessage[] messages = new IMessage[2];
            messages[0] = mocks.CreateMock<IMessage>();
            messages[1] = mocks.CreateMock<IMessage>();

            MessageQueueEndpoint returnTo = @"msmq://localhost/test_endpoint";

            IEnvelope e = new Envelope(returnTo, messages);

            Assert.That(e.Messages.Length, Is.EqualTo(2));
        }

        [Test]
        public void An_Param_Array_Of_Messages_Should_Be_Stored_In_The_Envelope()
        {
            MockRepository mocks = new MockRepository();

            IMessage message = mocks.CreateMock<IMessage>();
            IMessage message1 = mocks.CreateMock<IMessage>();

            MessageQueueEndpoint returnTo = @"msmq://localhost/test_endpoint";

            IEnvelope e = new Envelope(returnTo, message, message1);

            Assert.That(e.Messages.Length, Is.EqualTo(2));
        }

        [Test]
        public void The_SentTime_Should_Be_Set()
        {
            DateTime time = DateTime.Now;

            Envelope e = new Envelope();

            e.SentTime = time;

            Assert.That(e.SentTime, Is.EqualTo(time));
        }

        [Test]
        public void The_ArrivedTime_Should_Be_Set()
        {
            DateTime time = DateTime.Now;

            Envelope e = new Envelope();

            e.ArrivedTime = time;

            Assert.That(e.ArrivedTime, Is.EqualTo(time));
        }

        [Test]
        public void The_Id_Should_Be_Set()
        {
            MessageId id = Guid.NewGuid() + "\\27";

            Envelope e = new Envelope();

            e.Id = id;

            Assert.That(e.Id, Is.EqualTo(id));
        }

        [Test]
        public void The_CorrelationId_Should_Be_Set()
        {
            MessageId id = Guid.NewGuid() + "\\27";

            Envelope e = new Envelope();

            e.CorrelationId = id;

            Assert.That(e.CorrelationId, Is.EqualTo(id));
        }

        [Test]
        public void The_Label_Should_Be_Set()
        {
            string id = Guid.NewGuid().ToString();

            Envelope e = new Envelope();

            e.Label = id;

            Assert.That(e.Label, Is.EqualTo(id));
        }

        [Test]
        public void The_TimeToBeReceived_Should_Be_Set()
        {
            TimeSpan time = TimeSpan.FromMinutes(30);

            Envelope e = new Envelope();

            e.TimeToBeReceived = time;

            Assert.That(e.TimeToBeReceived, Is.EqualTo(time));
        }
    }
}