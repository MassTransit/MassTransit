namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using Magnum.TestFramework;


    [Scenario]
    public class CommittedTrxOnSend :
        GivenAChannel
    {
        private string theQueue = "testtrx";
                    byte[] theMessage = new byte[]{1,2,3};

        [When]
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

        [Then]
        public void ThereShouldBeAMessage()
        {
            WithChannel(model=>
                {
                    var msg = model.BasicGet(theQueue, true);
                    msg.Body.ShouldEqual(theMessage);
                    msg.MessageCount.ShouldEqual<uint>(0);
                });
        }
    }

    [Scenario]
    public class RollbackTrxOnSend :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [When]
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

        [Then]
        public void ThereShouldNotBeAMessage()
        {
            WithChannel(model=>
                {
                    var msg = model.BasicGet(theQueue, true);
                    msg.ShouldBeNull();
                });
        }
    }
    [Scenario]
    public class AckAGet :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [When]
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

        [Then]
        public void ThereShouldBeNoMoreMessages()
        {
            WithChannel(model =>
                {
                    var msg = model.BasicGet(theQueue, false);
                    msg.ShouldBeNull();
                });
            
        }
        
    }
    [Scenario]
    public class DontAckTheGet :
        GivenAChannel
    {
        private string theQueue = "testtrx";

        [When]
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

        [Then]
        public void ThereShouldBeAMessage()
        {
            WithChannel(model =>
                {
                    var msg = model.BasicGet(theQueue, false);
                    msg.ShouldNotBeNull();
                    msg.Body.ShouldEqual(TheMessage);
                });
            
        }
        
    }
}