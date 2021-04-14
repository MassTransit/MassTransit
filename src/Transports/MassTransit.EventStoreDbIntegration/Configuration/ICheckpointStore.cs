using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public interface ICheckpointStore
    {
        Task<ulong?> GetLastCheckpoint(CancellationToken cancellationToken = default);
        Task StoreCheckpoint(ulong? position, CancellationToken cancellationToken = default);
    }
}
