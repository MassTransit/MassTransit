namespace MassTransit.ServiceBus.Tests.Formatters
{
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_Json_Serializer
    {
        private MassTransit.ServiceBus.Formatters.IMessageFormatter _formatter;
        private MockRepository mocks;
        
        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            _formatter = new JsonMessageFormatter();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            _formatter = null;
        }

        [Test]
        public void Serialize()
        {
            ClientMessage msg = new ClientMessage();
            msg.Name = "dru";
            IFormattedBody body = mocks.CreateMock<IFormattedBody>();
            using(mocks.Record())
            {
                body.Body = "[{\"Name\":\"dru\"}]";
            }

            using(mocks.Playback())
            {
                _formatter.Serialize(body, msg);
            }
            
        }

        [Test]
        public void Deserialize()
        {
            ClientMessage msg = new ClientMessage();
            msg.Name = "dru";
            IFormattedBody body = mocks.CreateMock<IFormattedBody>();

            using (mocks.Record())
            {
                Expect.Call(body.Body).Return("[{\"Name\":\"dru\"}]");
            }

            using (mocks.Playback())
            {
                IMessage[] msgs = _formatter.Deserialize(body);
            }
        }
    }
}