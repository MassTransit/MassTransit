namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class LoadTests : 
        Specification
    {
        private IFormattedBody mockBody;
        private string _jsonBody = @"{""WrappedJson"":""{}"",""Types"":[""MassTransit.ServiceBus.Tests.PingMessage, MassTransit.ServiceBus.Tests""]}";
        private string _xmlBody = "<?xml version=\"1.0\"?>\r\n<anyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"PingMessage\" />";
        private int _iterations = 1000;
        private IBodyFormatter _formatter;

        protected override void Before_each()
        {
            mockBody = this.Mock<IFormattedBody>();
        }

        [Test]
        public void Xml()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_xmlBody);
            MemoryStream str = new MemoryStream(buffer);
            mockBody = new DumpMessageBody(str);

            _formatter = new XmlBodyFormatter();


            DateTime start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                str = new MemoryStream(buffer);
                str.Position = 0;
                mockBody = new DumpMessageBody(str);
                PingMessage msg = _formatter.Deserialize<PingMessage>(mockBody);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("{0} milliseconds", end.Subtract(start).Milliseconds);
        }

        [Test]
        public void Binary()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_xmlBody);
            Rhino.Mocks.SetupResult.For(mockBody.BodyStream).Return(new MemoryStream(buffer));

            _formatter = new BinaryBodyFormatter();

            for (int i = 0; i == _iterations; i++)
            {
                PingMessage msg = _formatter.Deserialize<PingMessage>(mockBody);
            }
        }

        [Test]
        public void Json()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_jsonBody);
            MemoryStream str = new MemoryStream(buffer);
            mockBody = new DumpMessageBody(str);

            _formatter = new JsonBodyFormatter();

            DateTime start = DateTime.Now;
            for(int i = 0; i < _iterations; i++)
            {
                str = new MemoryStream(buffer);
                str.Position = 0;
                mockBody = new DumpMessageBody(str);
                PingMessage msg = _formatter.Deserialize<PingMessage>(mockBody);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("{0} milliseconds", end.Subtract(start).Milliseconds);
        }

        public class DumpMessageBody : 
            IFormattedBody
        {
            private Stream _bodyStream;


            public DumpMessageBody(Stream bodyStream)
            {
                _bodyStream = bodyStream;
            }

            public object Body
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Stream BodyStream
            {
                get { return _bodyStream; }
                set { throw new NotImplementedException(); }
            }
        }
    }
}