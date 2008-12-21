namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Transactions;
    using Exceptions;
    using MassTransit.Tests;
    using Messages;
    using NUnit.Framework;
    using DeleteMessage=MassTransit.Tests.DeleteMessage;

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
            public void While_writing_it_should_perisist_on_complete()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    trx.Complete();
                }

                _ep.VerifyMessageInQueue<DeleteMessage>();
            }

            [Test]
            public void While_writing_it_should_perisist_even_on_rollback()
            {
                using (TransactionScope trx = new TransactionScope())
                {
                    _ep.Send(new DeleteMessage());
                    //no complete
                }

                _ep.VerifyMessageInQueue<DeleteMessage>();
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
            public void While_writing_it_should_persist()
            {
                _ep.Send(new DeleteMessage());

                _ep.VerifyMessageInQueue<DeleteMessage>();
            }

            [Test]
            public void While_reading_it_should_pull_object_from_queue()
            {
                _ep.Purge();
                _ep.Send(new VariableMessage("dru"));
                object obj = _ep.Receive(TimeSpan.FromSeconds(30));

                obj.ShouldNotBeNull();
                obj.ShouldBeSameType<VariableMessage>();
            }
        }

        [TestFixture]
        public class When_endpoint_doesnt_exist
        {
            [Test]
            [ExpectedException(typeof(EndpointException))]
            public void Should_throw_an_endpoint_exception()
            {
                new MsmqEndpoint("msmq://localhost/idontexist");
            }
        }

    }
}