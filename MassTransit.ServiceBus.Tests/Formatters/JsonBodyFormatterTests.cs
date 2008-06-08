namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System.IO;
    using System.Text;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    public class JsonBodyFormatterTests : 
        Specification
    {
        private JsonBodyFormatter formatter;
        private IFormattedBody mockBody;

        private readonly string _serializedMessages =
            @"{""WrappedJson"":""{}"",""Types"":[""MassTransit.ServiceBus.Tests.PingMessage, MassTransit.ServiceBus.Tests""]}";

        private readonly string _serializedMessagesWithValue =
             @"{""WrappedJson"":""{\""Name\"":\""test\""}"",""Types"":[""MassTransit.ServiceBus.Tests.ClientMessage, MassTransit.ServiceBus.Tests""]}";

    	protected override void Before_each()
        {
            formatter = new JsonBodyFormatter();
            mockBody = StrictMock<IFormattedBody>();
        }

        protected override void After_each()
        {
            formatter = null;
            mockBody = null;
        }

        [Test]
        public void Serialize()
        {
            PingMessage msg = new PingMessage();
            MemoryStream str = new MemoryStream();

            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(str);
            }

            using (Playback())
            {
                formatter.Serialize(mockBody, msg);
            }
            
            string output = StreamToString(str);
            Assert.That(output, Is.EqualTo(_serializedMessages));

        }

        [Test]
        public void Deserialize()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessages));
            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (Playback())
            {
                object msg = formatter.Deserialize(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(PingMessage)));
            }
        }

        [Test]
        public void DeserializeWithGenerics()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessages));
            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (Playback())
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

            MemoryStream ms = new MemoryStream();
            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }

            using (Playback())
            {
                formatter.Serialize(mockBody, msg);
            }

            string output = StreamToString(ms);
            Assert.That(output, Is.EqualTo(_serializedMessagesWithValue));
        }

        [Test]
        public void DeserializeObjectWithValues()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessagesWithValue));
            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (Playback())
            {
                object msg = formatter.Deserialize(mockBody);

                Assert.IsNotNull(msg);

                Assert.That(msg, Is.TypeOf(typeof(ClientMessage)));
            }
        }

        [Test]
        public void DeserializeObjectWithValuesWithGenerics()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_serializedMessagesWithValue));
            using (Record())
            {
                Expect.Call(mockBody.BodyStream).Return(ms);
            }
            using (Playback())
            {
                ClientMessage msg = formatter.Deserialize<ClientMessage>(mockBody);

                Assert.IsNotNull(msg);
                Assert.That(msg, Is.TypeOf(typeof(ClientMessage)));
                Assert.That(msg.Name, Is.EqualTo("test"));
            }
        }
        private static byte[] Convert(Stream str)
        {
            byte[] buffer = new byte[str.Length];
            str.Position = 0;
            str.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        private static string StreamToString(Stream str)
        {
            byte[] buffer = Convert(str);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}