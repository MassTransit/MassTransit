namespace MassTransit.ServiceBus.MSMQ.Tests
{
    using System.Transactions;
    using Exceptions;
    using Internal;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class When_receiving_a_message
    {
        string uri = "msmq://localhost/test_transactions";
        private MsmqEndpoint ep;
        private MsmqMessageReceiver mr;
        private Envelope env;
        private DeleteMessage msg;

        [SetUp]
        public void SetUp()
        {
            QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);
            ep = new MsmqEndpoint(uri);
            mr = new MsmqMessageReceiver(ep);

            msg = new DeleteMessage();
            env = new Envelope(msg);

            Put_a_test_message_on_the_queue();
        }

        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void From_A_Transactional_Queue_Without_a_transaction()
        {

        }

        [Test]
        public void From_A_Transactional_Queue_With_a_transaction()
        {

        }

        private void Put_a_test_message_on_the_queue()
        {
            MsmqMessageSender s = new MsmqMessageSender(ep);

            using (TransactionScope tr = new TransactionScope())
            {
                s.Send(env);
                tr.Complete();
            }
        }
    }
}