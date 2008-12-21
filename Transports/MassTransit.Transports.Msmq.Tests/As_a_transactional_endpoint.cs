namespace MassTransit.Transports.Msmq.Tests
{
    using System.Transactions;
    using Exceptions;
    using MassTransit.Tests;
    using NUnit.Framework;

    public class As_a_transactional_endpoint
    {
        [TestFixture]
        public class When_in_a_transaction
        {
            private MsmqEndpoint _ep;

            [SetUp]
            public void SetUp()
            {
                QueueTestContext.ValidateAndPurgeQueue(".\\private$\\mt_client_tx", true);
                _ep = new MsmqEndpoint("msmq://localhost/mt_client_tx");
            }

            [TearDown]
            public void TearDown()
            {
				_ep.Dispose();
                _ep = null;
            }


            [Test]
            public void While_writing_it_should_perisist_on_complete()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    trx.Complete();
                }


                QueueTestContext.VerifyMessageInTransactionalQueue(_ep, new DeleteMessage());
            }

            [Test]
            public void While_writing_it_should_not_perisist_on_rollback()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    //no complete
                }


                QueueTestContext.VerifyMessageNotInTransactionalQueue(_ep);
            }
        }

        [TestFixture]
        public class When_outside_a_transaction
        {
            private MsmqEndpoint _ep;

            [SetUp]
            public void SetUp()
            {
                QueueTestContext.ValidateAndPurgeQueue(".\\private$\\mt_client_tx", true);
                _ep = new MsmqEndpoint("msmq://localhost/mt_client_tx");
            }

            [TearDown]
            public void TearDown()
            {
				_ep.Dispose();
				_ep = null;
            }


            [Test]
            public void It_should_auto_enlist_a_transaction_and_persist()
            {
                _ep.Send(new DeleteMessage());

                QueueTestContext.VerifyMessageInTransactionalQueue(_ep, new DeleteMessage());
            }

        }

        [TestFixture]
        public class When_endpoint_doesnt_exist
        {
            [Test]
            [ExpectedException(typeof(EndpointException))]
            public void Should_throw_an_endpoint_exception()
            {
                new MsmqEndpoint("msmq://localhost/idontexist_tx");
            }
        }
        
    }
}