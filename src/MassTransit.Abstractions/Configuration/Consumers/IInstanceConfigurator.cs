namespace MassTransit
{
    public interface IInstanceConfigurator :
        IConsumeConfigurator
    {
    }


    public interface IInstanceConfigurator<TInstance> :
        IConsumerConfigurator<TInstance>,
        IInstanceConfigurator
        where TInstance : class, IConsumer
    {
    }
}
