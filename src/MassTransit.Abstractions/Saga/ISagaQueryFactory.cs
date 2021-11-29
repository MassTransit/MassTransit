namespace MassTransit
{
    /// <summary>
    /// Used to create a saga query from the message consume context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface ISagaQueryFactory<TSaga, in TMessage> :
        IProbeSite
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Creates a saga query from the specified message context
        /// </summary>
        /// <param name="context">The message context</param>
        /// <param name="query"></param>
        /// <returns></returns>
        bool TryCreateQuery(ConsumeContext<TMessage> context, out ISagaQuery<TSaga> query);
    }
}
