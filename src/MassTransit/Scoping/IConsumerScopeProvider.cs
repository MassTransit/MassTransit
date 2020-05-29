namespace MassTransit.Scoping
{
    using GreenPipes;


    /// <summary>
    /// Provides container scope for the consumer, either at the general level or the message-specific level.
    /// </summary>
    public interface IConsumerScopeProvider :
        IProbeSite
    {
        IConsumerScopeContext GetScope(ConsumeContext context);

        IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class;
    }
}
