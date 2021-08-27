using MassTransit.Transports.OnRamp.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public interface ISweeperRepository : IConnectionAndTransactionHolder, ILockRepository
    {
        // This will need to create a transaction, fetches messages and reserves them (if clustered = true)
        // This can have the transaction in a using statement, because it also doesn't have any fancy (create another transaction = true)
        // don't need to worry about sharing the connection, the sweeper is a self isolated service. the ISemaphore handles thread locking and db (if applicable)
        Task FailedToSendMessage(OnRampSweeperSendException exception, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OnRampMessage>> FetchNextMessages(string onRampName, int prefetchCount, CancellationToken cancellationToken = default);
        Task RemoveMessage(OnRampMessage message, CancellationToken cancellationToken = default);
        Task ReserveMessages(IEnumerable<Guid> enumerable, string onRampName, string instanceId, CancellationToken cancellationToken = default);
        Task RemoveAllMessages(string onRampName, string instanceId, CancellationToken cancellationToken = default);
        Task RemoveAllCompletedMessages(List<OnRampMessage> completedMessages, CancellationToken cancellationToken = default);
        Task FailedToSendMessages(List<OnRampMessage> failedMessages, string instanceId, CancellationToken cancellationToken = default);
    }
}
