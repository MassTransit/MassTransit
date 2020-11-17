using System.Data;

namespace MassTransit.Transports.Outbox
{
    public interface IOutboxDbTransactionContext
    {
        IDbTransaction Transaction { get; }
        void SetTransaction(IDbTransaction dbTransaction);
    }
}
