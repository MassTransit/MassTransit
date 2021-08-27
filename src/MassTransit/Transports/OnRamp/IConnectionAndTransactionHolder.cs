using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface IConnectionAndTransactionHolder
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(bool openNewTransaction = false, CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(bool transientError = false, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
