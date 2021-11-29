namespace MassTransit.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface SagaRepositoryContext<TSaga, TMessage> :
        ISagaConsumeContextFactory<TSaga>,
        ConsumeContext<TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Add the saga instance, using the specified <see cref="SagaConsumeContext{TSaga,T}" />
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance);

        /// <summary>
        /// Insert the saga instance, if it does not already exist.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>
        /// A valid <see cref="SagaConsumeContext{TSaga,T}" /> if the instance inserted successfully, otherwise default
        /// </returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance);

        /// <summary>
        /// Load an existing saga instance
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns>
        /// A valid <see cref="SagaConsumeContext{TSaga,T}" /> if the instance loaded successfully, otherwise default
        /// </returns>
        Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId);

        /// <summary>
        /// Save the saga, called after an Add, without an insert
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Save(SagaConsumeContext<TSaga> context);

        /// <summary>
        /// Update the saga, called after a load or insert where the saga has not completed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Update(SagaConsumeContext<TSaga> context);

        /// <summary>
        /// Delete the saga, called after a Load when the saga is completed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Delete(SagaConsumeContext<TSaga> context);

        /// <summary>
        /// Discard the saga, called after an Add when the saga is completed within the same transaction
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Discard(SagaConsumeContext<TSaga> context);

        /// <summary>
        /// Undo the changes for the saga
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Undo(SagaConsumeContext<TSaga> context);
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
