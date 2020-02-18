namespace MassTransit.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface SagaRepositoryContext<TSaga, TMessage> :
        ISagaConsumeContextFactory<TSaga>,
        ConsumeContext<TMessage>
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
    }


    public interface SagaRepositoryContext<TSaga> :
        PipeContext
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Query saga instances
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load an existing saga instance
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns>The saga, if found, or null</returns>
        Task<TSaga> Load(Guid correlationId);
    }
}
