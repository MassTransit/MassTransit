namespace MassTransit.ServiceBus.Tests
{
    using System;
    using Internal;
    using NUnit.Framework;

    [TestFixture]
    public class As_an_EndpointResolver
    {
        [SetUp]
        public void I_want_to()
        {
            
        }

        [Test]
        public void Be_intializable()
        {
            EndpointResolver res = new EndpointResolver();
            res.Initialize();
        }

        [Test]
        public void Be_able_to_resolve_endpoints()
        {
            EndpointResolver res = new EndpointResolver();
            res.Initialize();
            IEndpoint ep = res.Resolve(new Uri("msmq://localhost/test"));
            Assert.IsNotNull(ep);
        }
    }

    public class FakeSender : IMessageSender
    {
        public void Send(IEnvelope envelope)
        {
            //swallow
        }

        public void Dispose()
        {
            //swallow
        }
    }
    public class FakeReceiver : IMessageReceiver
    {
        public void Subscribe(IEnvelopeConsumer consumer)
        {
//swallow
        }

        public void Dispose()
        {
//swallow
        }
    }

    public class FakeEndpoint : IEndpoint
    {
        private Uri _uri;
        private IMessageSender _sender;
        private IMessageReceiver _receiver;

        public FakeEndpoint(Uri uri)
        {
            _uri = uri;
            _sender = new FakeSender();
            _receiver = new FakeReceiver();
        }


        public Uri Uri
        {
            get { return _uri; }
        }

        public IMessageSender Sender
        {
            get { return _sender; }
        }

        public IMessageReceiver Receiver
        {
            get { return _receiver; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}