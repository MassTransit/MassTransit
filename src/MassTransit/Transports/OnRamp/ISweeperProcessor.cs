using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface ISweeperProcessor
    {
        Task ExecuteAsync(Guid requestorId, CancellationToken cancellationToken);
    }
}
