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
        [Test]
        public void The_Message_Should_Be_Delivered_To_All_Subscribers()
        {
			bool _updated = false;
        	ManualResetEvent _updateEvent = new ManualResetEvent(false);

			_serviceBus.Subscribe<UpdateMessage>().MessageReceived += delegate
			{
				_updated = true;
				_updateEvent.Set();
			};

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
        }

        [Test]
        public void The_Message_Should_Be_Handled_By_A_Remote_Service()
        {
			bool _updated = false;
			ManualResetEvent _updateEvent = new ManualResetEvent(false);

			_remoteServiceBus.Subscribe<UpdateMessage>().MessageReceived += delegate
			{
				_updated = true;
				_updateEvent.Set();
			};

        	Thread.Sleep(TimeSpan.FromSeconds(5));

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
        }

        [Test]
        public void The_Message_Should_Be_Handled_By_A_Remote_Service_And_Replyable()
        {
            bool _updated = false;
            bool _replied = false;
            ManualResetEvent _updateEvent = new ManualResetEvent(false);

            EventHandler<MessageReceivedEventArgs<UpdateMessage>> handler = delegate(object sender, MessageReceivedEventArgs<UpdateMessage> args)
            {
                _updated = true;
                _updateEvent.Set();
                _remoteServiceBus.Send(args.Envelope.ReturnTo, new UpdateMessage());
            };

            _remoteServiceBus.Subscribe<UpdateMessage>().MessageReceived += handler;
            _serviceBus.Subscribe<UpdateMessage>().MessageReceived += delegate { _replied = true; };

            Thread.Sleep(TimeSpan.FromSeconds(5));

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
            Assert.That(_replied, Is.True, "Reply Failed");
        }

		[Test]
		public void Multiple_Local_Services_Should_Be_Available()
		{
			bool _updated = false;
			ManualResetEvent _updateEvent = new ManualResetEvent(false);

			_serviceBus.Subscribe<UpdateMessage>().MessageReceived += delegate
			{
				_updated = true;
				_updateEvent.Set();
			};

			bool _deleted = false;
			ManualResetEvent _deleteEvent = new ManualResetEvent(false);

			_serviceBus.Subscribe<DeleteMessage>().MessageReceived += delegate
			{
				_deleted = true;
				_deleteEvent.Set();
			};

			Thread.Sleep(TimeSpan.FromSeconds(5));

			DeleteMessage dm = new DeleteMessage();

			_serviceBus.Publish(dm);

			Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
						"Timeout expired waiting for message");

			Assert.That(_deleted, Is.True);

			UpdateMessage um = new UpdateMessage();

			_serviceBus.Publish(um);

			Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
						"Timeout expired waiting for message");

			Assert.That(_updated, Is.True);
		}

        [Test]
        public void Multiple_Remote_Services_Should_Be_Available()
        {
			bool _updated = false;
			ManualResetEvent _updateEvent = new ManualResetEvent(false);

			_remoteServiceBus.Subscribe<UpdateMessage>().MessageReceived += delegate
			{
				_updated = true;
				_updateEvent.Set();
			};

			bool _deleted = false;
			ManualResetEvent _deleteEvent = new ManualResetEvent(false);

			_remoteServiceBus.Subscribe<DeleteMessage>().MessageReceived += delegate
			{
				_deleted = true;
				_deleteEvent.Set();
			};

            Thread.Sleep(TimeSpan.FromSeconds(3));

            DeleteMessage dm = new DeleteMessage();

            _serviceBus.Publish(dm);

            Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_deleted, Is.True);

            UpdateMessage um = new UpdateMessage();

            _serviceBus.Publish(um);

            Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(12), true), Is.True,
                        "Timeout expired waiting for message");

            Assert.That(_updated, Is.True);
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