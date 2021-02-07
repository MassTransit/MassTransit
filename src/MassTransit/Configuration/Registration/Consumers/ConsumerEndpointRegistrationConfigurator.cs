namespace MassTransit.Registration
{
    public class ConsumerEndpointRegistrationConfigurator<TConsumer> :
        EndpointRegistrationConfigurator<TConsumer>,
        IConsumerEndpointRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
    }
}
