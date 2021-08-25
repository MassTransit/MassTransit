using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface ISweeperProcessor
    {
        Task ExecuteAsync(Guid requestorId, CancellationToken cancellationToken);
    }
}
