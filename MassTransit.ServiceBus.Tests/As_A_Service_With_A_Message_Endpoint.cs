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

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
            mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
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
                Expect.Call(mockEndpoint.QueueName).Return(@".\private$\test");
                Expect.Call(mockEndpoint.QueueName).Return(@".\private$\test");
                Expect.Call(mockEndpoint.Uri).Return(new Uri("msmq://localhost/test")).Repeat.Any(); //stupid log4net
                mockSubscriptionStorage.Add(typeof(PingMessage).FullName, new Uri("msmq://localhost/test"));
            }

            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
                _serviceBus.Subscribe<PingMessage>(delegate { });
            }
        }
    }
}