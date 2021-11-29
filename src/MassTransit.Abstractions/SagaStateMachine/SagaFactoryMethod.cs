namespace MassTransit
{
    /// <summary>
    /// Used to create the saga instance
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <param name="context">The message consume context</param>
    /// <returns>A newly created saga instance</returns>
    public delegate TSaga SagaFactoryMethod<out TSaga, in TMessage>(ConsumeContext<TMessage> context)
        where TSaga : class, ISaga
        where TMessage : class;
}
