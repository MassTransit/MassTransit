namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Pipeline_Ideas 
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            //pull message out endpoint
            PingMessage msg = new PingMessage();

            ParticularConsumer positiveConsumer = new ParticularConsumer(true);
            ParticularConsumer negativeConsumer = new ParticularConsumer(false);
            IndiscriminantConsumer indiscriminantConsumer = new IndiscriminantConsumer();

            ConsumerDispatcher<PingMessage> dispatcher = new ConsumerDispatcher<PingMessage>();

            dispatcher.Add(message => positiveConsumer.Accept(message) ? positiveConsumer : Consumes<PingMessage>.Null);
            dispatcher.Add(message => negativeConsumer.Accept(message) ? negativeConsumer : Consumes<PingMessage>.Null);
            dispatcher.Add(message => indiscriminantConsumer);

            for (int i = 0; i < 100000; i++)
                foreach (Consumes<PingMessage>.All item in dispatcher.GetConsumers(msg))
                    item.Consume(msg);
            

            Assert.AreEqual(positiveConsumer.Consumed, msg);
            Assert.AreEqual(positiveConsumer.Consumed, msg);
            Assert.AreEqual(negativeConsumer.Consumed, null);
        }
    }

    public class ConsumerDispatcher<TMessage> where TMessage : class
    {
        private readonly List<AcceptorEnumerator<TMessage>> _messageAcceptors = new List<AcceptorEnumerator<TMessage>>();

        public void Add(Func<TMessage, Consumes<TMessage>.All> acceptor)
        {
            _messageAcceptors.Add(new AcceptorEnumerator<TMessage>(acceptor));
        }

        public IEnumerable<Consumes<TMessage>.All> GetConsumers(TMessage message)
        {
            foreach (AcceptorEnumerator<TMessage> acceptor in _messageAcceptors)
            {
                foreach (Consumes<TMessage>.All consumer in acceptor.GetConsumers(message))
                {
                    yield return consumer;
                }
            }
        }
    }

    public class AcceptorEnumerator<TMessage>  where TMessage : class
    {
        private readonly Func<TMessage, Consumes<TMessage>.All> _acceptor;

        public AcceptorEnumerator(Func<TMessage, Consumes<TMessage>.All> acceptor)
        {
            _acceptor = acceptor;
        }

        public IEnumerable<Consumes<TMessage>.All> GetConsumers(TMessage message)
        {
            var consumer = _acceptor(message);

            if(consumer != null)
                yield return consumer;
        }
    }

    public class ParticularConsumer :
        Consumes<PingMessage>.Selected
    {
        private readonly bool _accept;
        private PingMessage _consumed;

        public ParticularConsumer(bool accept)
        {
            _accept = accept;
        }

        public PingMessage Consumed
        {
            get { return _consumed; }
        }

        public void Consume(PingMessage message)
        {
            _consumed = message;
        }

        public bool Accept(PingMessage message)
        {
            return _accept;
        }
    }

    public class IndiscriminantConsumer :
        Consumes<PingMessage>.All
    {
        private PingMessage _consumed;

        public PingMessage Consumed
        {
            get { return _consumed; }
        }

        public void Consume(PingMessage message)
        {
            _consumed = message;
        }
    }
}