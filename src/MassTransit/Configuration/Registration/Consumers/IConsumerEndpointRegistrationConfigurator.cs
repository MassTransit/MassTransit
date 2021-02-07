namespace MassTransit.Registration
{
    public interface IConsumerEndpointRegistrationConfigurator<TConsumer> :
        IEndpointRegistrationConfigurator
        where TConsumer : class, IConsumer
    {
    }
}
