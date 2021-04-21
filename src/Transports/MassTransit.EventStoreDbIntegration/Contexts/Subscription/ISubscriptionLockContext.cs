using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ISubscriptionLockContext
    {
        Task Complete(ResolvedEvent resolvedEvent);
        Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken);
    }
}
