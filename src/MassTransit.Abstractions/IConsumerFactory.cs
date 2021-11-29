namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Maps an instance of a consumer to one or more Consume methods for the specified message type
    /// The whole purpose for this interface is to allow the creator of the consumer to manage the lifecycle
    /// of the consumer, along with anything else that needs to be managed by the factory, container, etc.
    /// </summary>
    /// <typeparam name="TConsumer">The Consumer type</typeparam>
    public interface IConsumerFactory<out TConsumer> :
        IProbeSite
        where TConsumer : class
    {
        Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            where T : class;
    }
}
