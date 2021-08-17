namespace MassTransit.Registration
{
    using Saga;


    public class SagaEndpointRegistrationConfigurator<TSaga> :
        EndpointRegistrationConfigurator<TSaga>,
        ISagaEndpointRegistrationConfigurator
        where TSaga : class, ISaga
    {
    }
}
