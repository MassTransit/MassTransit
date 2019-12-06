namespace MassTransit
{
    using Registration;
    using Saga;


    public interface ISagaEndpointRegistrationConfigurator<TSaga> :
        IEndpointRegistrationConfigurator
        where TSaga : class, ISaga
    {
    }
}
