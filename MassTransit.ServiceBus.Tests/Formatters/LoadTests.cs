namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System;
    using System.IO;
    using System.Text;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class LoadTests :
        Specification
    {

        private string _jsonBody =
            @"{""WrappedJson"":""{}"",""Types"":[""MassTransit.ServiceBus.Tests.PingMessage, MassTransit.ServiceBus.Tests""]}";

        private readonly byte[] _binaryBody = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 12, 2, 0, 0, 0, 83, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 44, 32, 86, 101, 114, 115, 105, 111, 110, 61, 49, 46, 48, 46, 48, 46, 48, 44, 32, 67, 117, 108, 116, 117, 114, 101, 61, 110, 101, 117, 116, 114, 97, 108, 44, 32, 80, 117, 98, 108, 105, 99, 75, 101, 121, 84, 111, 107, 101, 110, 61, 110, 117, 108, 108, 5, 1, 0, 0, 0, 40, 77, 97, 115, 115, 84, 114, 97, 110, 115, 105, 116, 46, 83, 101, 114, 118, 105, 99, 101, 66, 117, 115, 46, 84, 101, 115, 116, 115, 46, 80, 105, 110, 103, 77, 101, 115, 115, 97, 103, 101, 0, 0, 0, 0, 2, 0, 0, 0, 11 };

        private int _iterations = 100000;
        private IBodyFormatter _formatter;
        private IFormattedBody _mockBody;

        protected override void Before_each()
        {
            _mockBody = this.DynamicMock<IFormattedBody>();
        }
        protected override void After_each()
        {
            _mockBody = null;
        }

        [Test]
        public void Binary()
        {
            byte[] buffer = _binaryBody;
            SetupResult.For(_mockBody.BodyStream).Return(new MemoryStream(buffer));
            ReplayAll();
            _formatter = new BinaryBodyFormatter();

            DateTime start = DateTime.Now;
            for (int i = 0; i == _iterations; i++)
            {
                PingMessage msg = _formatter.Deserialize<PingMessage>(_mockBody);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("{0} milliseconds", end.Subtract(start).Milliseconds);
        }

        [Test]
        [Ignore("Odd behaviour")]
        public void Json()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_jsonBody);
            SetupResult.For(_mockBody.BodyStream).Return(new MemoryStream(buffer));
            ReplayAll();
            _formatter = new JsonBodyFormatter();

            DateTime start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                PingMessage msg = _formatter.Deserialize<PingMessage>(_mockBody);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("{0} milliseconds", end.Subtract(start).Milliseconds);
        }
    }
}