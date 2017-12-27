namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    [TestFixture, Explicit, Category("SlowAF")]
    public class BasicAckIsFasterOnGet :
        GivenAChannel
    {
        private int HowMany = 100000;
        private string theQueue = "perf";
        private string theExchange = "perf";

        [SetUp]
        public void LoadUpMessages()
        {
            WithChannel(chan =>
            {
                chan.QueueDeclare(theQueue, false, false, false, null);
                chan.ExchangeDeclare(theExchange, "fanout", false);
                chan.QueueBind(theQueue, theExchange, "");

                for (int i = 0; i < HowMany; i++)
                {
                    chan.BasicPublish(theExchange, "", null, TheMessage);
                }
            });
        }

        [Test]
        public void NoAcks()
        {
            WithChannel(chan =>
            {
                WithStopWatch("NoAcks", () =>
                {
                    for (int i = 0; i < HowMany; i++)
                    {
                        var msg = chan.BasicGet(theQueue, true);
                    }
                });
            });
        }

        [Test]
        public void Acks()
        {
            WithChannel(chan =>
            {
                WithStopWatch("Acks", () =>
                {
                    for (int i = 0; i < HowMany; i++)
                    {
                        var msg = chan.BasicGet(theQueue, false);
                        chan.BasicAck(msg.DeliveryTag, false);
                    }
                });
            });
        }

        [Test]
        public void Transactions()
        {
            WithChannel(chan =>
            {
                WithStopWatch("Transactions", () =>
                {
                    for (int i = 0; i < HowMany; i++)
                    {
                        chan.TxSelect();
                        var msg = chan.BasicGet(theQueue, true);
                        chan.TxCommit();
                    }
                });
            });
        }

        [Test]
        public void BasicConsumer()
        {
            WithChannel(chan =>
            {
                WithStopWatch("BasicConsumer", () =>
                {
                    chan.BasicQos(0, 1000, false);

                    var consumer = new QueueingBasicConsumer(chan);
                    chan.BasicConsume(theQueue, false, consumer);
                    var queue = consumer.Queue;

                    for (int i = 0; i < HowMany; i++)
                    {
                        BasicDeliverEventArgs result;
                        queue.Dequeue(1000, out result);
                        chan.BasicAck(result.DeliveryTag, false);
                    }
                });
            });
        }
    }


    [TestFixture, Explicit, Category("SlowAF")]
    public class BasicPublishWithNoTxIsFaster :
        GivenAChannel
    {
        private int HowMany = 100000;
        private string theQueue = "perf";
        private string theExchange = "perf";

        [SetUp]
        public void LoadUpMessages()
        {
            WithChannel(chan =>
            {
                chan.QueueDeclare(theQueue, false, false, false, null);
                chan.ExchangeDeclare(theExchange, "fanout", false);
                chan.QueueBind(theQueue, theExchange, "");
                chan.QueuePurge(theQueue);
            });
        }

        [Test]
        public void NoTrx()
        {
            WithChannel(chan =>
            {
                WithStopWatch("publish no-trx", () =>
                {
                    for (int i = 0; i < HowMany; i++)
                    {
                        chan.BasicPublish(theExchange, "", null, TheMessage);
                    }
                });
            });
        }

        [Test]
        public void WithTrx()
        {
            WithChannel(chan =>
            {
                WithStopWatch("publish trx", () =>
                {
                    for (int i = 0; i < HowMany; i++)
                    {
                        chan.TxSelect();
                        chan.BasicPublish(theExchange, "", null, TheMessage);
                        chan.TxCommit();
                    }
                });
            });
        }
    }
}