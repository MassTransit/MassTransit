using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface IOnRampTransportRepository
    {
        // Only needs a transaction, and this can be externally owned (in fact most scenarios it likely is because of a using statement)
        Task InsertMessage(JsonSerializedMessage message, CancellationToken cancellationToken = default);
    }
}
