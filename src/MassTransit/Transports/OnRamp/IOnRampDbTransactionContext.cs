using System.Data;

namespace MassTransit.Transports.OnRamp
{
    public interface IOnRampDbTransactionContext
    {
        IDbTransaction Transaction { get; }
        void SetTransaction(IDbTransaction dbTransaction);
    }
}
