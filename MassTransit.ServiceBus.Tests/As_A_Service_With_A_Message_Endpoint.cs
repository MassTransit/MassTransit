using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using MassTransit.ServiceBus.Tests.Messages;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{

    [TestFixture]
	public class As_A_Service_With_A_Message_Endpoint
	{
        private ServiceBus _serviceBus;
        private MockRepository mocks;
        private IReadWriteEndpoint mockEndpoint;
        private ISubscriptionStorage mockSubscriptionStorage;


        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockEndpoint = mocks.CreateMock<IReadWriteEndpoint>();
            mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _serviceBus = null;
        }

		[Test]
		public void I_Want_To_Be_Able_To_Register_An_Event_Handler_For_Messages()
		{
            using(mocks.Record())
            {
                mockEndpoint.Subscribe(null);
                LastCall.IgnoreArguments();
                mockSubscriptionStorage.Add(typeof(PingMessage), mockEndpoint);
            }

            using(mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
                _serviceBus.Subscribe<PingMessage>(MyUpdateMessage_Received);
            }

            //What is this testing for?
            //Assert.That(_serviceBus.Consumer<PingMessage>(), Is.Not.Null);
		}

        private static void MyUpdateMessage_Received(MessageContext<PingMessage> ctx)
		{
		}
	}
}