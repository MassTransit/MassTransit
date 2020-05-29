namespace MassTransit
{
    public interface IConsumerTestFactorySelector<out TConsumer>
        where TConsumer : class, IConsumer
    {
        IConsumerFactory<TConsumer> ConsumerFactory { get; }
    }
}
