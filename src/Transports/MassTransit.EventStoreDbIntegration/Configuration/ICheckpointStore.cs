using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{

    public interface ICheckpointStore
    {
        Task<ulong?> GetCheckpoint();
        Task StoreCheckpoint(ulong? position);
    }
}
