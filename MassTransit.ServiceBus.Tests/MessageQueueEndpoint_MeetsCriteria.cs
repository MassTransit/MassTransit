using System;
using System.IO;
using System.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using MassTransit.ServiceBus.Subscriptions;

namespace MassTransit.ServiceBus.Tests
{
    using Internal;

    [TestFixture]
    public class MessageQueueEndpoint_MeetsCriteria
    {
        private MockRepository mocks;
        private ServiceBus _serviceBus;
        private IMessageQueueEndpoint mockServiceBusEndPoint;
        private IMessageReceiver mockReceiver;
        private string queueName = @".\private$\test_servicebus";

        [SetUp]
        public void SetUp()
        {
            ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);

            mocks = new MockRepository();
            mockServiceBusEndPoint = mocks.CreateMock<IMessageQueueEndpoint>();
            mockReceiver = mocks.CreateMock<IMessageReceiver>();
        }
        [TearDown]
        public void TearDown()
        {
            mocks = null;
            mockServiceBusEndPoint = null;
            _serviceBus = null;
        }

        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Not_Be_Handled()
        {
            using(mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockServiceBusEndPoint, new LocalSubscriptionCache());
                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                IEnvelopeConsumer consumer = _serviceBus;

                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.False);
            }
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled()
        {
            using(mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockServiceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return true; });

                IEnvelopeConsumer consumer = _serviceBus;

                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
            }
        }

        [Test]
        public void dru_test()
        {
            bool workDid = false;

            using (mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockServiceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { workDid = true; },
                    delegate { return true; });

                IEnvelopeConsumer consumer = _serviceBus;

                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
                consumer.Deliver(envelope);
                Assert.That(workDid, Is.True, "Lazy Test!");
            }
        }

        [Test]
        public void The_Service_Bus_Should_Return_True_If_The_Message_Will_Be_Handled_By_One_Of_multiple_handlers()
        {
            using(mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
				Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockServiceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return true; });

                IEnvelopeConsumer consumer = _serviceBus;

                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.True);
            }
        }
        
        [Test]
        public void The_Service_Bus_Should_Return_False_If_The_Message_Will_Be_Handled_By_none_Of_the_handlers()
        {
            using(mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver).Repeat.Any();
				Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
                Expect.Call(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockServiceBusEndPoint, new LocalSubscriptionCache());

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                _serviceBus.Subscribe<PingMessage>(
                    delegate { },
                    delegate { return false; });

                IEnvelopeConsumer consumer = _serviceBus;

                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());

                Assert.That(consumer.IsHandled(envelope), Is.False);
            }
        }

        [Test]
        [Ignore("Uber Nasty")]
        public void The_Message_Endpoint_Should_Check_If_The_Message_Will_Be_Handled()
        {
            IEndpoint mockReturnEndpoint = mocks.CreateMock<IEndpoint>();
            IEnvelopeConsumer mockConsumer = mocks.CreateMock<IEnvelopeConsumer>();
            IMessageQueueEndpoint mockMessageQueueEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();

            IEnvelope envelope = new Envelope(mockReturnEndpoint, new PingMessage());
            Message msg = EnvelopeMessageMapper.MapFrom(envelope);
            IMsmqQueue mockQueue = mocks.CreateMock<IMsmqQueue>();
            Cursor mockCursor = null;

            using(mocks.Record())
            {
                Expect.Call(mockMessageQueueEndpoint.Open(QueueAccessMode.SendAndReceive)).Return(mockQueue);

                Expect.Call(mockQueue.CreateCursor()).Return(mockCursor).Repeat.Any();
                Expect.Call(mockQueue.Peek(TimeSpan.FromSeconds(1), mockCursor, PeekAction.Current)).Return(msg).Repeat.Any();

                //Expect.Call(mockMessageQueueEndpoint.Uri).Return(new Uri("msmq://localhost/test_servicebus")).Repeat.Any(); //stupid log4net
                //Expect.Call(mockQueue.CreateCursor()).Return(null);
                //Expect.Call(mockQueue.BeginPeek(TimeSpan.FromHours(24), null, PeekAction.Current, this, null)).IgnoreArguments().Return(null);
                //Expect.Call(mockConsumer.IsHandled(envelope)).Return(false);
            }

            using (mocks.Playback())
            {
                IMessageReceiver receiver = new MessageQueueReceiver(mockMessageQueueEndpoint);
                receiver.Subscribe(mockConsumer);

                Message queueMessage = EnvelopeMessageMapper.MapFrom(envelope);
                queueMessage.BodyStream.Seek(0, SeekOrigin.Begin);

                ((MessageQueueReceiver)receiver).ProcessMessage(queueMessage);
            }
        }

        [Test]
        public void NAME()
        {
            string input = @"FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\test_servicebus";
            string output = ServiceBusSetupFixture.GetQueueName(input);
            Assert.AreEqual(@".\private$\test_servicebus", output);
        }
    }
}