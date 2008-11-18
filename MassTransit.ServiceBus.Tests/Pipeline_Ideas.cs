namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Pipeline_Ideas :
        LocalAndRemoteTestContext
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            Container.AddComponent<ParticularConsumer>();

            //LocalBus.Subscribe<ParticularConsumer>();


            //pull message out endpoint
            PingMessage msg = new PingMessage();

            ParticularConsumer positiveConsumer = new ParticularConsumer(true);
            ParticularConsumer negativeConsumer = new ParticularConsumer(false);
            IndiscriminantConsumer indiscriminantConsumer = new IndiscriminantConsumer();
            
            EnumerableDispatcher<PingMessage> dispatcher = new EnumerableDispatcher<PingMessage>(msg)
                {
                    (message) => positiveConsumer.Accept(message) ? positiveConsumer : null,
                    (message) => negativeConsumer.Accept(message) ? negativeConsumer : null,
                    (message) => indiscriminantConsumer,
                    //Add
                };

            IEnumerable<Consumes<PingMessage>.All> consumers = dispatcher;

            foreach (Consumes<PingMessage>.All item in consumers)
            {
                item.Consume(msg);
            }

            Assert.AreEqual(positiveConsumer.Consumed, msg);
            Assert.AreEqual(positiveConsumer.Consumed, msg);
            Assert.AreEqual(negativeConsumer.Consumed, null);
        }
    }

    public class EnumerableDispatcher<TMessage> : IEnumerable<Consumes<TMessage>.All> where TMessage : class
    {
        private readonly TMessage _message;
        private readonly List<AcceptorEnumerator<TMessage>> _messageAcceptors = new List<AcceptorEnumerator<TMessage>>();


        public EnumerableDispatcher(TMessage message)
        {
            _message = message;
        }

        public void Add(Func<TMessage, Consumes<TMessage>.All> acceptor)
        {
            _messageAcceptors.Add(new AcceptorEnumerator<TMessage>(acceptor));
        }

        public IEnumerator<Consumes<TMessage>.All> GetEnumerator()
        {
            foreach (AcceptorEnumerator<TMessage> acceptor in _messageAcceptors)
            {
                foreach (Consumes<TMessage>.All consumer in acceptor.GetConsumers(_message))
                {
                    yield return consumer;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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