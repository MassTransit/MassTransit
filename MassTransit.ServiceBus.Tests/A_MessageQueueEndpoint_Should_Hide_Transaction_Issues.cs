using System;
using System.Transactions;
using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests
{
    using Rhino.Mocks;

    [Explicit]
    [TestFixture(Description = "Integration Test for Transaction Handling")]
    public class A_MessageQueueEndpoint_Should_Hide_Transaction_Issues
    {
        private readonly string nonTransactionalQueueName = @".\private$\test_nonTransaction";
        private readonly string transactionalQueueName = @".\private$\test_transaction";
        private readonly string returnToQueueName = @".\private$\test_return";

        private MockRepository mocks;
        IEndpoint returnTo;
        PingMessage msg = new PingMessage();
        Envelope env;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            ServiceBusSetupFixture.ValidateAndPurgeQueue(nonTransactionalQueueName);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(returnToQueueName);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(transactionalQueueName, true);
            returnTo = mocks.CreateMock<IEndpoint>();
            
            env = new Envelope(returnTo, msg);
        }
        [TearDown]
        public void TearDown()
        {
            mocks = null;
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional()
        {
            using(mocks.Record())
            {
                Expect.Call(returnTo.Address).Return(returnToQueueName);
            }
            using(mocks.Playback())
            {
                MessageQueueEndpoint ep = nonTransactionalQueueName;
                MessageSenderFactory.Create(ep).Send(env);
            }

            ServiceBusSetupFixture.VerifyMessageInQueue(nonTransactionalQueueName, msg);
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
             using(mocks.Record())
            {
                Expect.Call(returnTo.Address).Return(returnToQueueName);
            }
            using (mocks.Playback())
            {
                using (TransactionScope tr = new TransactionScope())
                {
                    MessageQueueEndpoint ep = nonTransactionalQueueName;
                    MessageSenderFactory.Create(ep).Send(env);

                    tr.Complete();
                }

                using (TransactionScope tr = new TransactionScope())
                {
                    ServiceBusSetupFixture.VerifyMessageInQueue(nonTransactionalQueueName, msg);

                    tr.Complete();
                }
            }
        }


        [Test]
        public void When_The_Queue_Is_Transactional()
        { using(mocks.Record())
            {
                Expect.Call(returnTo.Address).Return(returnToQueueName);
            }
            using (mocks.Playback())
            {
                using (TransactionScope tr = new TransactionScope())
                {
                    MessageQueueEndpoint ep = transactionalQueueName;
                    MessageSenderFactory.Create(ep).Send(env);

                    tr.Complete();
                }


                using (TransactionScope tr = new TransactionScope())
                {
                    ServiceBusSetupFixture.VerifyMessageInQueue(transactionalQueueName, msg);

                    tr.Complete();
                }
            }
        }


        [Test]
        [ExpectedException(typeof(Exception))]
        public void When_The_Queue_Is_Transactional_Not_In_A_Transaction()
        {
             using(mocks.Record())
            {
                Expect.Call(returnTo.Address).Return(returnToQueueName);
            }
            using (mocks.Playback())
            {
                MessageQueueEndpoint ep = transactionalQueueName;
                MessageSenderFactory.Create(ep).Send(env);
            }
        }
        
    }
}