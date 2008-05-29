namespace MassTransit.ServiceBus.Tests.Formatters
{
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class JsonBodyFormatterTests
    {
        private MockRepository mocks;
        private JsonBodyFormatter formatter;
        private IFormattedBody mockBody;

        private readonly string _serializedMessages =
            @"{""WrappedJson"":""{}"",""Types"":[""MassTransit.ServiceBus.Tests.PingMessage""]}";

        private readonly string _serializedMessagesWithValue =
             @"{""WrappedJson"":""{\""Name\"":\""test\""}"",""Types"":[""MassTransit.ServiceBus.Tests.ClientMessage""]}";

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            formatter = new JsonBodyFormatter();
            mockBody = mocks.CreateMock<IFormattedBody>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            formatter = null;
            mockBody = null;
        }

        [Test]
        public void Serialize()
        {
            PingMessage msg = new PingMessage();

            using (mocks.Record())
            {
                mockBody.Body = _serializedMessages;
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
        }

        [Test]
        public void Deserialize()
        {
            using (mocks.Record())
            {
                Expect.Call(mockBody.Body).Return(_serializedMessages);
            }
            using (mocks.Playback())
            {
                PingMessage msg = formatter.Deserialize<PingMessage>(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void SerializeObjectWithValues()
        {
            ClientMessage msg = new ClientMessage();
            msg.Name = "test";


            using (mocks.Record())
            {
                mockBody.Body = _serializedMessagesWithValue;
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
        }
    }
}