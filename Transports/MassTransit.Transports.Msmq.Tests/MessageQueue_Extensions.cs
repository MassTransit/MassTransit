namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using System.Transactions;

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
    }
}