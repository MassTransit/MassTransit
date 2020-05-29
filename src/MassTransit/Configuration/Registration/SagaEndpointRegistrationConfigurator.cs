namespace MassTransit.Registration
{
    using Saga;


    public class SagaEndpointRegistrationConfigurator<TSaga> :
        EndpointRegistrationConfigurator<TSaga>,
        ISagaEndpointRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
    }
}
