namespace MassTransit.Saga
{
    using System;
    using System.Threading.Tasks;


    public interface SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Add the saga instance, using the specified <see cref="SagaConsumeContext{TSaga,T}"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance);

        /// <summary>
        /// Insert the saga instance, if it does not already exist.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>A valid <see cref="SagaConsumeContext{TSaga,T}"/> if the instance inserted successfully, otherwise default</returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance);

        /// <summary>
        /// Load an existing saga instance
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns>A valid <see cref="SagaConsumeContext{TSaga,T}"/> if the instance loaded successfully, otherwise default</returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId);

        /// <summary>
        /// Query saga instances
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<SagaRepositoryQueryContext<TSaga, TMessage>> Query(ISagaQuery<TSaga> query);

        /// <summary>
        /// Called if the saga operation throws an exception, allowing transactions to be rolled back, etc.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task Faulted(Exception exception);
    }


    public interface SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Query saga instances
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query);

        /// <summary>
        /// Load an existing saga instance
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns>The saga, if found, or null</returns>
        Task<TSaga> Load(Guid correlationId);

        /// <summary>
        /// Called if the saga operation throws an exception, allowing transactions to be rolled back, etc.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task Faulted(Exception exception);
    }
}
