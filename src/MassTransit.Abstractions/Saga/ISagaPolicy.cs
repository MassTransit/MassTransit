namespace MassTransit
{
    using System.Threading.Tasks;


    public interface ISagaPolicy<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// If true, changes should not be saved to the saga repository
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// If true, the instance returned should be used to try and insert as a new saga instance, ignoring any failures
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instance"></param>
        /// <returns>True if the instance should be inserted before invoking the message logic</returns>
        bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance);

        /// <summary>
        /// The method invoked when an existing saga instance is present
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Existing(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next);

        /// <summary>
        /// Invoked when there is not an existing saga instance available
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next);
    }
}
