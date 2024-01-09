#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ConnectionContext :
        PipeContext
    {
        Uri HostAddress { get; }

        string? Schema { get; }

        IsolationLevel IsolationLevel { get; }

        ClientContext CreateClientContext(CancellationToken cancellationToken);

        /// <summary>
        /// Create a database connection
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ISqlTransportConnection> CreateConnection(CancellationToken cancellationToken);

        Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a query within a transaction using an available connection
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> Query<T>(Func<IDbConnection, IDbTransaction, Task<T>> callback, CancellationToken cancellationToken);
    }
}
