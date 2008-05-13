using System;
using System.IO;
using System.Text;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Formatters;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.Patterns.Tests
{
    [TestFixture]
    public class XmlMessageFormatterTests
    {
        private MockRepository mocks;
        private XmlBodyFormatter formatter;
        private IFormattedBody mockBody;

        private Guid _id = new Guid("6C1769FE-D8E0-4b9c-832C-79E668141C66");

        private readonly string _serializedMessages =
            "<?xml version=\"1.0\"?>\r\n<ArrayOfAnyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <anyType xsi:type=\"BatchMessageToBatch\">\r\n    <Body>\r\n      <Name>dru</Name>\r\n    </Body>\r\n  </anyType>\r\n</ArrayOfAnyType>";


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
            MessageToBatch msg = new MessageToBatch();
            msg.Name = "dru";
            BatchMessageToBatch bm = new BatchMessageToBatch(_id, 1, msg);

            MemoryStream memoryStream = new MemoryStream();

            using (mocks.Record())
            {
                Expect.Call(mockBody.BodyStream).Return(memoryStream);
            }

            using (mocks.Playback())
            {
                formatter.Serialize(mockBody, bm);

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
                IMessage[] msgs = formatter.Deserialize<IMessage[]>(mockBody);

                Assert.AreEqual(1, msgs.Length);

                Assert.That(msgs[0], Is.TypeOf(typeof(BatchMessageToBatch)));

                BatchMessageToBatch bm = msgs[0] as BatchMessageToBatch;
                Assert.AreEqual("dru", bm.Body.Name);
                Assert.AreEqual(1, bm.BatchLength);
                Assert.AreEqual(_id, bm.BatchId);
            }
        }
    }
}