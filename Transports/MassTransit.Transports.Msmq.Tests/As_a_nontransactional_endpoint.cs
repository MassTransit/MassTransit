namespace MassTransit.Transports.Msmq.Tests
{
    using System.Transactions;
    using MassTransit.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class As_a_nontransactional_endpoint
    {

        [TestFixture]
        public class When_in_a_transaction
        {
            private MsmqEndpoint _ep;

            [SetUp]
            public void SetUp()
            {
                QueueTestContext.ValidateAndPurgeQueue(".\\private$\\mt_client", true);
                _ep = new MsmqEndpoint("msmq://localhost/mt_client");
            }

            [TearDown]
            public void TearDown()
            {
            	_ep.Dispose();
                _ep = null;
            }


            [Test]
            public void While_writing_it_should_perisist_on_success()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    trx.Complete();
                }

                using (TransactionScope trx = new TransactionScope())
                {
                    QueueTestContext.VerifyMessageInQueue(_ep, new DeleteMessage());
                    trx.Complete();
                }
            }

            [Test]
            public void While_writing_it_should_perisist_even_on_failure()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    //no complete
                }


                QueueTestContext.VerifyMessageInQueue(_ep, new DeleteMessage());
            }
        }

        [TestFixture]
        public class When_outside_a_transaction
        {
            private MsmqEndpoint _ep;

            [SetUp]
            public void SetUp()
            {
                QueueTestContext.ValidateAndPurgeQueue(".\\private$\\mt_client", true);
                _ep = new MsmqEndpoint("msmq://localhost/mt_client");
            }

            [TearDown]
            public void TearDown()
            {
				_ep.Dispose();
                _ep = null;
            }


            [Test]
            public void It_should_persist()
            {
                _ep.Send(new DeleteMessage());

                QueueTestContext.VerifyMessageInQueue(_ep, new DeleteMessage());
            }

        }

    }
}