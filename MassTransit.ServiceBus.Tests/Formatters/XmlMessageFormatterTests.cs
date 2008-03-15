using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class XmlMessageFormatterTests
    {
        private MockRepository mocks;
        private XmlBodyFormatter formatter;
        IFormattedBody mockBody;

		private readonly string _serializedMessages = "<?xml version=\"1.0\"?>\r\n<ArrayOfAnyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <anyType xsi:type=\"PingMessage\" />\r\n</ArrayOfAnyType>";

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
                IMessage[] msgs = formatter.Deserialize(mockBody);

                Assert.AreEqual(1, msgs.Length);

            	Assert.That(msgs[0], Is.TypeOf(typeof (PingMessage)));
            }
        }
    }
}