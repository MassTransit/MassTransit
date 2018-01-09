namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;



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