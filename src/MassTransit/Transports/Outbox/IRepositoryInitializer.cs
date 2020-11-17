using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface IRepositoryInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
