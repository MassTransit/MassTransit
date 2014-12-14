namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using NUnit.Framework;
    using Shouldly;


    public class CommittedTrxOnSend :
        GivenAChannel
    {
        private string theQueue = "testtrx";
                    byte[] theMessage = new byte[]{1,2,3};

        [SetUp]
        public void TrxOnSend()
        {
            WithChannel(model=>
                {
                    model.QueueDeclare(theQueue, true, false, false, null);
                    model.ExchangeDeclare("trx", "fanout", true, true, null);
                    model.QueueBind(theQueue, "trx", "");
                    model.QueuePurge(theQueue);
                });

            WithChannel(model=>
                {
                    model.TxSelect();
                    var props = model.CreateBasicProperties();
                    
                    model.BasicPublish("trx", "", props, theMessage);

                    model.TxCommit();
                });
        }

        [Test]
        public void ThereShouldBeAMessage()
        {
            WithChannel(model=>
                {
                    var msg = model.BasicGet(theQueue, true);
                    msg.Body.ShouldBe(theMessage);
                    msg.MessageCount.ShouldBe<uint>(0);
                });
        }
    }

    
    public class RollbackTrxOnSend :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [SetUp]
        public void TrxOnSend()
        {
            WithChannel(model=>
                {
                    model.QueueDeclare(theQueue, true, false, false, null);
                    model.ExchangeDeclare("trx", "fanout", true, true, null);
                    model.QueueBind(theQueue, "trx", "");
                    model.QueuePurge(theQueue);
                });

            WithChannel(model=>
                {
                    model.TxSelect();
                    var props = model.CreateBasicProperties();
                    
                    model.BasicPublish("trx", "", props, TheMessage);

                    model.TxRollback();
                });
        }

        [Test]
        public void ThereShouldNotBeAMessage()
        {
            WithChannel(model=>
                {
                    var msg = model.BasicGet(theQueue, true);
                    msg.ShouldBe(null);
                });
        }
    }
    
    public class AckAGet :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [SetUp]
        public void WeAckTheBasicGet()
        {
            WithChannel(model=>
                {
                    model.QueueDeclare(theQueue, true, false, false, null);
                    model.ExchangeDeclare("trx", "fanout", true, true, null);
                    model.QueueBind(theQueue, "trx", "");
                    model.QueuePurge(theQueue);

                    model.BasicPublish("trx","",model.CreateBasicProperties(), TheMessage);
                });

            WithChannel(chan=>
                {
                    var x = chan.BasicGet(theQueue, false);
                    chan.BasicAck(x.DeliveryTag, true);
                });
        }

        [Test]
        public void ThereShouldBeNoMoreMessages()
        {
            WithChannel(model =>
                {
                    var msg = model.BasicGet(theQueue, false);
                    msg.ShouldBe(null);
                });
            
        }
        
    }
    
    public class DontAckTheGet :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [SetUp]
        public void WeAckTheBasicGet()
        {
            WithChannel(model=>
                {
                    model.QueueDeclare(theQueue, true, false, false, null);
                    model.ExchangeDeclare("trx", "fanout", true, true, null);
                    model.QueueBind(theQueue, "trx", "");
                    model.QueuePurge(theQueue);

                    model.BasicPublish("trx","",model.CreateBasicProperties(), TheMessage);
                });

            WithChannel(chan=>
                {
                    var x = chan.BasicGet(theQueue, false);
                    //uh oh, no ack
                    //chan.BasicAck(x.DeliveryTag, true);
                });
        }

        [Test]
        public void ThereShouldBeAMessage()
        {
            WithChannel(model =>
                {
                    var msg = model.BasicGet(theQueue, false);
                    msg.ShouldNotBe(null);
                    msg.Body.ShouldBe(TheMessage);
                });
            
        }
        
    }
}