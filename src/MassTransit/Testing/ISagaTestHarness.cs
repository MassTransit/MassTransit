namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public interface ISagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        IReceivedMessageList Consumed { get; }
        ISagaList<TSaga> Sagas { get; }
        ISagaList<TSaga> Created { get; }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<Guid?> Exists(Guid correlationId, TimeSpan? timeout = default);

        /// <summary>
        /// Waits until at least one saga exists matching the specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, TimeSpan? timeout = default);

        /// <summary>
        /// Waits until the saga matching the specified correlationId does NOT exist
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<Guid?> NotExists(Guid correlationId, TimeSpan? timeout = default);
    }
}
