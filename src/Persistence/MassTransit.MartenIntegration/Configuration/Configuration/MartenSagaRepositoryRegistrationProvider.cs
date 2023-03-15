namespace MassTransit.Configuration
{
    public class MartenSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly bool _optimisticConcurrency;

        public MartenSagaRepositoryRegistrationProvider(bool optimisticConcurrency = false)
        {
            _optimisticConcurrency = optimisticConcurrency;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.MartenRepository(schema =>
            {
                if (_optimisticConcurrency)
                    schema.UseOptimisticConcurrency(true);
            });
        }
    }
}
