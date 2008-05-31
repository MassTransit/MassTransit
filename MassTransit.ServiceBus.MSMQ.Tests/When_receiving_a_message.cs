namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System.Transactions;
	using Exceptions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_receiving_a_message
	{
		private MockRepository mocks;
		private string uri = "msmq://localhost/test_transactions";
		private MsmqEndpoint ep;
		private DeleteMessage msg;

		private void Put_a_test_message_on_the_queue()
		{
			using (TransactionScope tr = new TransactionScope())
			{
				ep.Send(msg);
				tr.Complete();
			}
		}

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			ep = new MsmqEndpoint(uri);
			QueueTestContext.ValidateAndPurgeQueue(ep.QueuePath, true);

			msg = new DeleteMessage();

			Put_a_test_message_on_the_queue();
		}

		[Test]
		public void From_A_Transactional_Queue_With_a_transaction()
		{
			using (TransactionScope tr = new TransactionScope())
			{
				ep.Receive();
				tr.Complete();
			}
		}

		[Test]
		[ExpectedException(typeof (EndpointException))]
		public void From_A_Transactional_Queue_Without_a_transaction()
		{	
			ep.Receive();
		}
	}
}