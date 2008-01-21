using System;
using System.Transactions;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests.IntegrationTests
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
            ServiceBusSetupFixture.ValidateAndPurgeQueue(new MessageQueueEndpoint(nonTransactionalQueueName).QueuePath);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(new MessageQueueEndpoint(returnToQueueName).QueuePath);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(new MessageQueueEndpoint(transactionalQueueName).QueuePath, true);
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
            }
            using (mocks.Playback())
            {
                MessageQueueEndpoint ep = nonTransactionalQueueName;
                ep.Sender.Send(env);

                ServiceBusSetupFixture.VerifyMessageInQueue(new MessageQueueEndpoint(nonTransactionalQueueName).QueuePath, msg);
            }
        }

        [Test]
        public void When_The_Queue_Is_NonTransactional_In_A_Transaction()
        {
            using (mocks.Record())
            {
            }
            using (mocks.Playback())
            {
                using (TransactionScope tr = new TransactionScope())
                {
                    MessageQueueEndpoint ep = nonTransactionalQueueName;
                    ep.Sender.Send(env);

                    tr.Complete();
                }

                using (TransactionScope tr = new TransactionScope())
                {
                    ServiceBusSetupFixture.VerifyMessageInQueue(new MessageQueueEndpoint(nonTransactionalQueueName).QueuePath, msg);

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
                    ep.Sender.Send(env);

                    tr.Complete();
                }


                using (TransactionScope tr = new TransactionScope())
                {
                    ServiceBusSetupFixture.VerifyMessageInQueue(new MessageQueueEndpoint(transactionalQueueName).QueuePath, msg);

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
                ep.Sender.Send(env);
            }
        }
    }
}