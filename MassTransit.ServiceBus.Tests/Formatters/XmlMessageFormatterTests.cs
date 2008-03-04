using System;
using MassTransit.ServiceBus.Formatters;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests.Formatters
{
    using NUnit.Framework;

    [TestFixture]
    public class XmlMessageFormatterTests
    {
        MockRepository mocks = new MockRepository();
        private string serialized = "<?xml version=\"1.0\"?>" + Environment.NewLine + "<PingMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />";
        
        
        [Test]
        public void Serialize()
        {
            PingMessage msg = new PingMessage();
            XmlMessageFormatter xmf = new XmlMessageFormatter();
            IFormattedBody mockBody = mocks.CreateMock<IFormattedBody>();

            using(mocks.Record())
            {
                mockBody.Body = serialized;
            }
            using(mocks.Playback())
            {
                xmf.Serialize(mockBody, msg);
            }
        }

        [Test]
        public void Deserialize()
        {
            PingMessage msg = new PingMessage();
            XmlMessageFormatter xmf = new XmlMessageFormatter();
            IFormattedBody mockBody = mocks.CreateMock<IFormattedBody>();

            using (mocks.Record())
            {
                Expect.Call(mockBody.Body).Return(serialized);
            }
            using (mocks.Playback())
            {
                IMessage[] msgs = xmf.Deserialize(mockBody);
            }
        }
    }
}