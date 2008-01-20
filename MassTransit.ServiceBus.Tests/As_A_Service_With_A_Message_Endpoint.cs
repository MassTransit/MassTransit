using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class As_A_Service_With_A_Message_Endpoint
    {
        private IServiceBus _serviceBus;
        private MockRepository mocks;
        private IMessageQueueEndpoint mockEndpoint;
        private ISubscriptionStorage mockSubscriptionStorage;

        private string queueName = @".\private$\test";
        private Uri queueUri = new Uri("msmq://localhost/test");
        private IMessageReceiverFactory mockReceiverFactory;
        private IMessageSenderFactory mockSenderFactory;
        private IMessageReceiver mockReceiver;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
            mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
            mockReceiverFactory = mocks.CreateMock<IMessageReceiverFactory>();
            mockSenderFactory = mocks.CreateMock<IMessageSenderFactory>();
            mockReceiver = mocks.CreateMock<IMessageReceiver>();

            ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _serviceBus = null;
            mockEndpoint = null;
            mockSubscriptionStorage = null;
        }

        #endregion

        [Test]
        public void I_Want_To_Be_Able_To_Register_An_Event_Handler_For_Messages()
        {
            using (mocks.Record())
            {
                Expect.Call(mockReceiverFactory.Using(mockEndpoint)).Return(mockReceiver);
                Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments();
                Expect.Call(mockEndpoint.Uri).Return(queueUri).Repeat.Any(); //stupid log4net
                mockSubscriptionStorage.Add(typeof(PingMessage).FullName, queueUri);
            }

            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockEndpoint, mockSubscriptionStorage, mockSenderFactory, mockReceiverFactory);
                _serviceBus.Subscribe<PingMessage>(delegate { });
            }
        }
    }
}