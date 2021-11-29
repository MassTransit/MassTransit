namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// A saga repository is used by the service bus to dispatch messages to sagas
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaRepository<TSaga> :
        IProbeSite
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Send the message to the saga repository where the context.CorrelationId has the CorrelationId
        /// of the saga instance.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message consume context</param>
        /// <param name="policy">The saga policy for the message</param>
        /// <param name="next">The saga consume pipe</param>
        /// <returns></returns>
        Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class;

        /// <summary>
        /// Send the message to the saga repository where the query is used to find matching saga instances,
        /// which are invoked concurrently.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The saga query consume context</param>
        /// <param name="query"></param>
        /// <param name="policy">The saga policy for the message</param>
        /// <param name="next">The saga consume pipe</param>
        /// <returns></returns>
        Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class;
    }
}
