using MassTransit.Transports.Outbox.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface ISweeperRepository : IConnectionAndTransactionHolder, ILockRepository
    {
        // This will need to create a transaction, fetches messages and reserves them (if clustered = true)
        // This can have the transaction in a using statement, because it also doesn't have any fancy (create another transaction = true)
        // don't need to worry about sharing the connection, the sweeper is a self isolated service. the ISemaphore handles thread locking and db (if applicable)
        Task FailedToSendMessage(OutboxSweeperSendException exception, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OutboxMessage>> FetchNextMessages(string outboxName, int prefetchCount, CancellationToken cancellationToken = default);
        Task RemoveMessage(OutboxMessage message, CancellationToken cancellationToken = default);
        Task ReserveMessages(IEnumerable<Guid> enumerable, string outboxName, string instanceId, CancellationToken cancellationToken = default);
        Task RemoveAllMessages(string outboxName, string instanceId, CancellationToken cancellationToken = default);
        Task RemoveAllCompletedMessages(List<OutboxMessage> completedMessages, CancellationToken cancellationToken = default);
        Task FailedToSendMessages(List<OutboxMessage> failedMessages, string instanceId, CancellationToken cancellationToken = default);
    }
}
