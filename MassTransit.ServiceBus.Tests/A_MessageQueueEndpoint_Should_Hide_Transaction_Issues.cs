using System;
using System.Transactions;
using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class A_MessageQueueEndpoint_Should_Hide_Transaction_Issues
        : ServiceBusSetupFixture
    {
        private readonly string nonTransactionalQueueName = @".\private$\test_nonTransaction";
        private readonly string transactionalQueueName = @".\private$\test_transaction";

        IEndpoint returnTo;
        TransactionalTestMessage msg = new TransactionalTestMessage();
        Envelope env;

        public override void Before_Each_Test_In_The_Fixture()
        {
            base.Before_Each_Test_In_The_Fixture();
            ValidateAndPurgeQueue(nonTransactionalQueueName);
            ValidateAndPurgeQueue(transactionalQueueName, true);

            returnTo = new MessageQueueEndpoint(_testEndPoint);
            env = new Envelope(returnTo, msg);
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional()
        {
            MessageQueueEndpoint ep = new MessageQueueEndpoint(nonTransactionalQueueName);
            ep.Send(env);

            VerifyMessageInQueue(nonTransactionalQueueName, msg);
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MessageQueueEndpoint ep = new MessageQueueEndpoint(nonTransactionalQueueName);
                ep.Send(env);

                tr.Complete();
            }

            VerifyMessageInQueue(nonTransactionalQueueName, msg);
        }


        [Test]
        public void When_The_Queue_Is_Transactional()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MessageQueueEndpoint ep = new MessageQueueEndpoint(transactionalQueueName);
                ep.Send(env);

                tr.Complete();
            }

            VerifyMessageInQueue(transactionalQueueName, msg);
        }


        [Test]
        [Ignore("What do we want to do here?")]
        [ExpectedException("No Transaction You Fool Exception")]
        public void When_The_Queue_Is_Transactional_Not_In_A_Transaction()
        {
            MessageQueueEndpoint ep = new MessageQueueEndpoint(transactionalQueueName);
            ep.Send(env);
        }
        
    }

    [Serializable]
    public class TransactionalTestMessage : IMessage {}
}