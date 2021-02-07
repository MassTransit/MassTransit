namespace MassTransit.Registration
{
    using Saga;


    public interface ISagaRepositoryRegistrationProvider
    {
        void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga;
    }
}
