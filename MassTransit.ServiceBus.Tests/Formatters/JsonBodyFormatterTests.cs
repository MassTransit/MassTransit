namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System.IO;
    using System.Text;
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
            @"{""WrappedJson"":""{}"",""Types"":[""MassTransit.ServiceBus.Tests.PingMessage, MassTransit.ServiceBus.Tests""]}";

        private readonly string _serializedMessagesWithValue =
             @"{""WrappedJson"":""{\""Name\"":\""test\""}"",""Types"":[""MassTransit.ServiceBus.Tests.ClientMessage, MassTransit.ServiceBus.Tests""]}";

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
                Expect.Call(mockBody.BodyStream).Return(new MemoryStream());
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
        }

        [Test]
        public void Deserialize()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessages));
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (mocks.Playback())
            {
                PingMessage msg = formatter.Deserialize<PingMessage>(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void DeserializeWithOutGenerics()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessages));
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (mocks.Playback())
            {
                object msg = formatter.Deserialize(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void SerializeObjectWithValues()
        {
            ClientMessage msg = new ClientMessage();
            msg.Name = "test";

            MemoryStream ms = new MemoryStream();
            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
        }
    }
}