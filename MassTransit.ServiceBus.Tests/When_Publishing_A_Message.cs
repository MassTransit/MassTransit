using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_Publishing_A_Message :
        ServiceBusSetupFixture
    {
        private volatile bool _updated = false;
        private readonly ManualResetEvent _updateEvent = new ManualResetEvent(false);

        private volatile bool _deleted = false;
        private readonly ManualResetEvent _deleteEvent = new ManualResetEvent(false);

        private void UpdateMessage_Received(object sender, MessageReceivedEventArgs<UpdateMessage> e)
        {
            _updated = true;
            _updateEvent.Set();
        }

        private void DeleteMessage_Received(object sender, MessageReceivedEventArgs<DeleteMessage> e)
        {
            _deleted = true;
            _deleteEvent.Set();
        }

        [Test]
        public void The_Message_Should_Be_Delivered_To_All_Subscribers()
        {
            _updated = false;
            _updateEvent.Reset();

            _serviceBus.Endpoint<UpdateMessage>().MessageReceived += UpdateMessage_Received;

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
        }

        [Test]
        public void The_Message_Should_Be_Handled_By_A_Remote_Service()
        {
            _updated = false;
            _updateEvent.Reset();

            _remoteServiceBus.Endpoint<UpdateMessage>().MessageReceived += UpdateMessage_Received;

            Thread.Sleep(TimeSpan.FromSeconds(3));

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
        }

        [Test]
        public void Multiple_Services_Should_Be_Available()
        {
            _remoteServiceBus.Endpoint<UpdateMessage>().MessageReceived += UpdateMessage_Received;
            _remoteServiceBus.Endpoint<DeleteMessage>().MessageReceived += DeleteMessage_Received;

            Thread.Sleep(TimeSpan.FromSeconds(3));

            DeleteMessage dm = new DeleteMessage();

            _serviceBus.Publish(dm);

            Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_deleted, Is.True);

            //UpdateMessage um = new UpdateMessage();

            //_serviceBus.Publish(um);

            //Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
            //            "Timeout expired waiting for message");

            //Assert.That(_updated, Is.True);
        }

    }

    [Serializable]
    public class UpdateMessage : IMessage
    {
    }

    [Serializable]
    public class DeleteMessage : IMessage
    {
    }
}