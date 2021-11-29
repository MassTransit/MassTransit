namespace MassTransit.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaRepositoryContextFactory<TSaga> :
        IProbeSite
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a <see cref="SagaRepositoryContext{TSaga,T}" /> and send it to the next pipe.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class;

        /// <summary>
        /// Create a <see cref="SagaRepositoryQueryContext{TSaga,T}" /> and send it to the next pipe.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query"></param>
        /// <param name="next"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class;

        /// <summary>
        /// Create a <see cref="SagaRepositoryContext{TSaga}" /> and send it to the next pipe.
        /// </summary>
        /// <param name="asyncMethod"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class;
    }
}
