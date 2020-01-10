namespace MassTransit.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaRepositoryContextFactory<TSaga> :
        IProbeSite
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a <see cref="SagaRepositoryContext{T}"/>
        /// </summary>
        /// <param name="context">The <see cref="ConsumeContext{T}"/> to scope</param>
        /// <param name="correlationId">The correlationId of the saga, which may be locked by the factory</param>
        /// <returns></returns>
        Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId = default)
            where T : class;

        /// <summary>
        /// Create a <see cref="SagaRepositoryContext{T}"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default);
    }
}
