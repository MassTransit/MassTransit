namespace MassTransit.Persistence.Configuration
{
    using Integration.Saga;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class CustomRepositoryConfigurator<TSaga> : ICustomRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly List<Action<IServiceCollection>> _callbacks = new();

        DatabaseContextFactory<TSaga>? _contextFactory;

        /// <inheritdoc />
        public ICustomRepositoryConfigurator<TSaga> SetContextFactory(DatabaseContextFactory<TSaga> contextFactory)
        {
            _contextFactory = contextFactory;
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate()
        {
            if (_contextFactory is null)
                yield return this.Failure("ContextFactory must be set");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> services)
        {
            _callbacks.ForEach(c => c.Invoke(services));
            _callbacks.Clear();

            services.AddSingleton(_ => _contextFactory!);

            services.RegisterLoadSagaRepository<TSaga, CustomSagaRepositoryContextFactory<TSaga>>();
            services.RegisterQuerySagaRepository<TSaga, CustomSagaRepositoryContextFactory<TSaga>>();
            services
                .RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                    CustomSagaRepositoryContextFactory<TSaga>>();
        }

        public void AddCallback(Action<IServiceCollection> callback)
        {
            _callbacks.Add(callback);
        }
    }
}
