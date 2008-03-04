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
        private XmlMessageFormatter formatter;
        IFormattedBody mockBody;

        private readonly string serialized = "<?xml version=\"1.0\"?>" + Environment.NewLine +
                                    "<PingMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />";

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            formatter = new XmlMessageFormatter();
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
                mockBody.Body = serialized;
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
                Expect.Call(mockBody.Body).Return(serialized);
            }
            using (mocks.Playback())
            {
                IMessage[] msgs = formatter.Deserialize(mockBody);
                Assert.AreEqual(1, msgs.Length);
            }
        }
    }
}