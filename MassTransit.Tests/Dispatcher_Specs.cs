namespace MassTransit.Tests
{
    using System;
    using System.Collections;
    using Configuration;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Internal;
    using MassTransit.Serialization;
    using MassTransit.Subscriptions;
    using MassTransit.Transports;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    
    using Tests.Messages;
    using TextFixtures;

    [TestFixture]
    public class When_a_message_is_delivered_to_the_service_bus :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void A_consumer_object_should_receive_the_message()
        {
            FutureMessage<PingMessage> fm = new FutureMessage<PingMessage>();
            PingHandler handler = new PingHandler(fm);

            LocalBus.Subscribe(handler);

            int old = PingHandler.Pinged;

            RemoteBus.Publish(new PingMessage());
            fm.IsAvailable(1.Seconds());
            Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
        }

        [Test]
        public void A_consumer_type_should_be_created_to_receive_the_message()
        {
            FutureMessage<PingMessage> fm = new FutureMessage<PingMessage>();
            PingHandler ph = new PingHandler(fm);

        	ObjectBuilder.Stub(x => x.GetInstance<PingHandler>()).Return(ph);
			ObjectBuilder.Stub(x => x.GetInstance<PingHandler>(new Hashtable())).IgnoreArguments().Return(ph);


            LocalBus.Subscribe<PingHandler>();

            int old = PingHandler.Pinged;

            RemoteBus.Publish(new PingMessage());
            fm.IsAvailable(1.Seconds());
            Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
        }


        internal class PingHandler : Consumes<PingMessage>.All
        {
            private readonly FutureMessage<PingMessage> _fm;
            private static int _pinged;

            public PingHandler(FutureMessage<PingMessage> fm)
            {
                _fm = fm;
            }

            public static int Pinged
            {
                get { return _pinged; }
            }

            public void Consume(PingMessage message)
            {
                _pinged++;
                _fm.Set(message);
            }
        }
    }
}