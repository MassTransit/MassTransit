namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Creates a saga instance when an existing saga instance is missing
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface ISagaFactory<out TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Create a new saga instance using the supplied consume context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        TSaga Create(ConsumeContext<TMessage> context);

        /// <summary>
        /// Send the context through the factory, with the proper decorations
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Send(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next);
    }
}
