namespace MassTransit.Transports.RabbitMq.Tests.Assumptions
{
    using NUnit.Framework;

    [TestFixture]
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
                    chan.ExchangeDeclare(theExchange, "fanout",false);
                    chan.QueueBind(theQueue,theExchange, "");

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
            WithChannel(chan=>
                {
                    WithStopWatch("Acks", ()=>
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
    }



    [TestFixture]
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