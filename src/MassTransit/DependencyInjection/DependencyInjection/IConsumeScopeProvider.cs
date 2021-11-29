namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;


    /// <summary>
    /// Provides container scope for the consumer, either at the general level or the message-specific level.
    /// </summary>
    public interface IConsumeScopeProvider :
        IProbeSite
    {
        ValueTask<IConsumeScopeContext> GetScope(ConsumeContext context);

        ValueTask<IConsumeScopeContext<T>> GetScope<T>(ConsumeContext<T> context)
            where T : class;

        ValueTask<IConsumerConsumeScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class;
    }
}
