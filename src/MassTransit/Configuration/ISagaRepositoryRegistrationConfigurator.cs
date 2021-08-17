namespace MassTransit
{
    using Registration;
    using Saga;


    public interface ISagaRepositoryRegistrationConfigurator<TSaga> :
        IContainerRegistrar
        where TSaga : class, ISaga
    {
    }
}
