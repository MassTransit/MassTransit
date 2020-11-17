using System.Data;

namespace MassTransit.Transports.Outbox
{
    public class OutboxDbTransactionContext : IOutboxDbTransactionContext
    {
        public IDbTransaction Transaction { get; private set; }

        public void SetTransaction(IDbTransaction dbTransaction)
        {
            Transaction = dbTransaction;
        }
    }
}
