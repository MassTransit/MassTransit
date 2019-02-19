namespace MassTransit.ConsumeConfigurators
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
