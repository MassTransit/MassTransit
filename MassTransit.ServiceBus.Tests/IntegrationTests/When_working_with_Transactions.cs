using System;
using System.Transactions;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
    [Explicit]
    [TestFixture(Description = "Integration Test for Transaction Handling")]
    public class When_working_with_Transactions
    {
        #region Setup/Teardown

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

        #endregion

        private readonly string nonTransactionalQueueName = @"msmq://localhost/test_nonTransaction";
        private readonly string transactionalQueueName = @"msmq://localhost/test_transaction";
        private readonly string returnToQueueName = @"msmq://localhost/test_return";

        private MockRepository mocks;
        private IEndpoint returnTo;
        private PingMessage msg = new PingMessage();
        private Envelope env;

        [Test]
        public void When_The_Queue_Is_NonTransactional()
        {
            using (mocks.Record())
            {
                Expect.Call(returnTo.Uri).Return(new Uri(returnToQueueName));
            }
            using (mocks.Playback())
            {
                MessageQueueEndpoint ep = nonTransactionalQueueName;
                MessageSenderFactory.Create(ep).Send(env);
            }

            ServiceBusSetupFixture.VerifyMessageInQueue(nonTransactionalQueueName, msg);
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
            using (mocks.Record())
            {
                Expect.Call(returnTo.Uri).Return(new Uri(returnToQueueName));
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
        {
            using (mocks.Record())
            {
                Expect.Call(returnTo.Uri).Return(new Uri(returnToQueueName));
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
        [ExpectedException(typeof (Exception))]
        public void When_The_Queue_Is_Transactional_Not_In_A_Transaction()
        {
            using (mocks.Record())
            {
                Expect.Call(returnTo.Uri).Return(new Uri(returnToQueueName));
            }
            using (mocks.Playback())
            {
                MessageQueueEndpoint ep = transactionalQueueName;
                MessageSenderFactory.Create(ep).Send(env);
            }
        }
    }
}