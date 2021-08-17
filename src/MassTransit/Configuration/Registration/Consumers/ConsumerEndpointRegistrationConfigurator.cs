namespace MassTransit.Registration
{
    public class ConsumerEndpointRegistrationConfigurator<TConsumer> :
        EndpointRegistrationConfigurator<TConsumer>,
        IConsumerEndpointRegistrationConfigurator
        where TConsumer : class, IConsumer
    {
    }
}
