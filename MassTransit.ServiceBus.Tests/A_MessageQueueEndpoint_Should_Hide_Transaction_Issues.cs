using System;
using System.Transactions;
using NUnit.Framework;
using MassTransit.ServiceBus.Tests.Messages;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class A_MessageQueueEndpoint_Should_Hide_Transaction_Issues
        : ServiceBusSetupFixture
    {
        private readonly string nonTransactionalQueueName = @".\private$\test_nonTransaction";
        private readonly string transactionalQueueName = @".\private$\test_transaction";

        IEndpoint returnTo;
        PingMessage msg = new PingMessage();
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
        [Ignore("Broken")]
        public void When_The_Queue_Is_NonTransactional()
        {
            MessageQueueEndpoint ep = new MessageQueueEndpoint(nonTransactionalQueueName);
            ep.Send(env);

            VerifyMessageInQueue(nonTransactionalQueueName, msg);
        }

        [Test]
        [Ignore("Broken")]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MessageQueueEndpoint ep = new MessageQueueEndpoint(nonTransactionalQueueName);
                ep.Send(env);

                tr.Complete();
            }

            using (TransactionScope tr = new TransactionScope())
            {
                VerifyMessageInQueue(nonTransactionalQueueName, msg);
                
                tr.Complete();
            }
        }


        [Test]
        [Ignore("Broken")]
        public void When_The_Queue_Is_Transactional()
        {
            using (TransactionScope tr = new TransactionScope())
            {
                MessageQueueEndpoint ep = new MessageQueueEndpoint(transactionalQueueName);
                ep.Send(env);

                tr.Complete();
            }


            using (TransactionScope tr = new TransactionScope())
            {
                VerifyMessageInQueue(transactionalQueueName, msg);

                tr.Complete();
            }
        }


        [Test]
        [ExpectedException(typeof(Exception))]
        public void When_The_Queue_Is_Transactional_Not_In_A_Transaction()
        {
            MessageQueueEndpoint ep = new MessageQueueEndpoint(transactionalQueueName);
            ep.Send(env);
        }
        
    }
}