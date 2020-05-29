namespace MassTransit.Saga
{
    using System.Threading.Tasks;


    /// <summary>
    /// Creates the <see cref="SagaConsumeContext{TSaga,T}" /> as needed by the <see cref="SagaRepositoryContext{TSaga}" />.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaConsumeContextFactory<in TContext, TSaga>
        where TContext : class
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a new <see cref="SagaConsumeContext{TSaga,T}" />.
        /// </summary>
        /// <param name="context">The <see cref="SagaRepositoryContext{TSaga}" /></param>
        /// <param name="consumeContext">The message consume context being delivered to the saga</param>
        /// <param name="instance">The saga instance</param>
        /// <param name="mode">The creation mode of the saga instance</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(TContext context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class;
    }


    /// <summary>
    /// Creates the <see cref="SagaConsumeContext{TSaga,T}" /> as needed by the <see cref="SagaRepositoryContext{TSaga}" />.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaConsumeContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a new <see cref="SagaConsumeContext{TSaga,T}" />.
        /// </summary>
        /// <param name="consumeContext">The message consume context being delivered to the saga</param>
        /// <param name="instance">The saga instance</param>
        /// <param name="mode">The creation mode of the saga instance</param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class;
    }
}
