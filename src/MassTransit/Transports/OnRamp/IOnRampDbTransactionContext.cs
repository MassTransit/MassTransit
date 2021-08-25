using System.Data;

namespace MassTransit.Transports.Outbox
{
    public interface IOnRampDbTransactionContext
    {
        IDbTransaction Transaction { get; }
        void SetTransaction(IDbTransaction dbTransaction);
    }
}
