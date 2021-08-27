using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface IRepositoryInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
