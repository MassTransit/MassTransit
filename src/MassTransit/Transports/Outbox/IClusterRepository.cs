using MassTransit.Transports.Outbox.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface IClusterRepository : IConnectionAndTransactionHolder, ILockRepository
    {
        // The cluster manager will need to have the ability to create a new transaction, so unlikely we can have using statement
        Task FreeAllMessagesFromAnySweeperInstance(string outboxName, CancellationToken cancellationToken = default);
        Task FreeMessagesFromFailedSweeperInstance(string outboxName, string instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OutboxSweeper>> GetAllSweepers(string outboxName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetMessageSweeperInstanceIds(string outboxName, CancellationToken cancellationToken = default);
        Task InsertSweeper(string outboxName, string instanceId, DateTime lastCheckin, TimeSpan checkinInterval, CancellationToken cancellationToken = default);
        Task RemoveSweeper(string outboxName, string instanceId, CancellationToken cancellationToken = default);
        Task<bool> UpdateSweeper(string outboxName, string instanceId, DateTime lastCheckin, CancellationToken cancellationToken = default);
    }
}
