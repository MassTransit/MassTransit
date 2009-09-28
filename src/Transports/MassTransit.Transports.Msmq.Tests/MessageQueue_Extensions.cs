namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using System.Transactions;
    using Internal;

	public static class MessageQueue_Extensions
    {
        public static void WithinTransaction(this MessageQueue queue, Action<MessageQueue, MessageQueueTransactionType> action)
        {
            if (queue.Transactional)
            {
                MessageQueueTransactionType tt = MessageQueueTransactionType.Automatic;

                TransactionScope transaction = new TransactionScope();
                try
                {
                    action(queue, tt);
                    transaction.Complete();
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            else
            {
                action(queue, MessageQueueTransactionType.None);
            }
        }

		public static long GetMessageCount(this IMsmqEndpointAddress address)
		{
			IEndpointManagement management = MsmqEndpointManagement.New(address.Uri);
			return management.Count();
		}
    }
}