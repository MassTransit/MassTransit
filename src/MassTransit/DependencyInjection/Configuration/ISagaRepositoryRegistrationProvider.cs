namespace MassTransit.Configuration
{
    public interface ISagaRepositoryRegistrationProvider
    {
        void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga;
    }
}
