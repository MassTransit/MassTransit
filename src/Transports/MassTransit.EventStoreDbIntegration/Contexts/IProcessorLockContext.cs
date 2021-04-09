using System.Threading.Tasks;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IProcessorLockContext
    {
        Task Complete(ResolvedEvent resolvedEvent);
    }
}
