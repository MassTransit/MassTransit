namespace MassTransit.MSMQ.Tests
{
    using System.Transactions;
    using Exceptions;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class When_sending_a_message
    {
        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void To_A_Transactional_Queue_Without_a_transaction()
        {
            string uri = "msmq://localhost/mt_client_tx";
            MsmqEndpoint ep = new MsmqEndpoint(uri);
            QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);

        	ep.Send(new DeleteMessage());
        }

        [Test]
        public void To_A_Transactional_Queue_With_a_transaction()
        {
            string uri = "msmq://localhost/mt_client_tx";
            MsmqEndpoint ep = new MsmqEndpoint(uri);
            QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);
         
			using(TransactionScope tr = new TransactionScope())
            {
                ep.Send(new DeleteMessage());
                tr.Complete();
            }

            QueueTestContext.VerifyMessageInQueue(ep.QueuePath, new DeleteMessage());
        }
    }
}