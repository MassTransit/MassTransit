namespace MassTransit.ServiceBus.MSMQ.Tests
{
    using System.Transactions;
    using Exceptions;
    using Messages;
    using NUnit.Framework;

    [Explicit]
    [TestFixture(Description = "Integration Test for Transaction Handling")]
    public class When_working_with_Transactions
    {
        private readonly string nonTransactionalQueueName = @"msmq://localhost/test_nonTransaction";
        private readonly string transactionalQueueName = @"msmq://localhost/test_transaction";
        private readonly string returnToQueueName = @"msmq://localhost/test_return";

        private DeleteMessage msg = new DeleteMessage();

        [SetUp]
        public void SetUp()
        {
            QueueTestContext.ValidateAndPurgeQueue(new MsmqEndpoint(nonTransactionalQueueName).QueuePath);
            QueueTestContext.ValidateAndPurgeQueue(new MsmqEndpoint(returnToQueueName).QueuePath);
            QueueTestContext.ValidateAndPurgeQueue(new MsmqEndpoint(transactionalQueueName).QueuePath, true);
        }


        [Test]
        public void When_The_Queue_Is_NonTransactional()
        {
            MsmqEndpoint ep = nonTransactionalQueueName;
            ep.Send(msg);

            QueueTestContext.VerifyMessageInQueue(new MsmqEndpoint(nonTransactionalQueueName).QueuePath, msg);
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MsmqEndpoint ep = nonTransactionalQueueName;
                ep.Send(msg);

                tr.Complete();
            }

            using (TransactionScope tr = new TransactionScope())
            {
                QueueTestContext.VerifyMessageInQueue(new MsmqEndpoint(nonTransactionalQueueName).QueuePath, msg);

                tr.Complete();
            }
        }


        [Test]
        public void When_The_Queue_Is_Transactional()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MsmqEndpoint ep = transactionalQueueName;
                ep.Send(msg);

                tr.Complete();
            }


            using (TransactionScope tr = new TransactionScope())
            {
                QueueTestContext.VerifyMessageInQueue(new MsmqEndpoint(transactionalQueueName).QueuePath, msg);

                tr.Complete();
            }
        }


        [Test]
        [ExpectedException(typeof (EndpointException))]
        public void When_The_Queue_Is_Transactional_Not_In_A_Transaction()
        {
            MsmqEndpoint ep = transactionalQueueName;
            ep.Send(msg);
        }
    }
}