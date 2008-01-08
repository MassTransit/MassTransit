using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class As_A_Service_With_A_Message_Endpoint
    {
        private IServiceBus _serviceBus;
        private MockRepository mocks;
        private IEndpoint mockEndpoint;
        private ISubscriptionStorage mockSubscriptionStorage;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockEndpoint = mocks.CreateMock<IEndpoint>();
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
            //using (mocks.Record())
            //{
            //    Expect.Call(mockEndpoint.Address).Return("bob").Repeat.Any(); //stupid log4net
            //    mockEndpoint.Subscribe(null);
            //    LastCall.IgnoreArguments();
            //    mockSubscriptionStorage.Add(typeof(PingMessage), mockEndpoint);
            //}

            //using (mocks.Playback())
            //{
            //    _serviceBus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
            //    _serviceBus.Subscribe<PingMessage>(delegate { });
            //}
        }
    }
}