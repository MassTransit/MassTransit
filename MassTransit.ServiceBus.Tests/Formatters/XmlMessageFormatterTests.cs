namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System.IO;
    using System.Text;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class XmlMessageFormatterTests
    {
        private MockRepository mocks;
        private XmlBodyFormatter formatter;
        private IFormattedBody mockBody;

        private readonly string _serializedMessages =
            "<?xml version=\"1.0\"?>\r\n<anyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"PingMessage\" />";

        private readonly string _serializedMessagesWithValue =
            "<?xml version=\"1.0\"?>\r\n<anyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"ClientMessage\">\r\n  <Name>test</Name>\r\n</anyType>";

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            formatter = new XmlBodyFormatter();
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

            MemoryStream memoryStream = new MemoryStream();

            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(memoryStream);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);

                byte[] buffer = new byte[memoryStream.Length];
                memoryStream.Position = 0;
                memoryStream.Read(buffer, 0, buffer.Length);

                string s = Encoding.UTF8.GetString(buffer);

                Assert.That(s, Is.EqualTo(_serializedMessages));
            }
        }

        [Test]
        public void Deserialize()
        {
            MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(_serializedMessages));

            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(memoryStream);
            }
            using (mocks.Playback())
            {
                object msg = formatter.Deserialize<object>(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof (PingMessage)));
            }
        }

        [Test]
        public void SerializeObjectWithValues()
        {
            ClientMessage msg = new ClientMessage();
            msg.Name = "test";

            MemoryStream memoryStream = new MemoryStream();

            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(memoryStream);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, msg);

                byte[] buffer = new byte[memoryStream.Length];
                memoryStream.Position = 0;
                memoryStream.Read(buffer, 0, buffer.Length);

                string s = Encoding.UTF8.GetString(buffer);

                Assert.That(s, Is.EqualTo(_serializedMessagesWithValue));
            }
        }
    }
}