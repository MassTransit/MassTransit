namespace MassTransit.MartenIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Marten;
    using Marten.Schema.Identity;
    using MassTransit.Saga;
    using Npgsql;
    using Saga.Context;


    public class MartenSagaRepositoryConfigurator<TSaga> :
        IMartenSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        Action<StoreOptions> _configureOptions;

        public void Connection(string connectionString, Action<StoreOptions> configure = null)
        {
            void ConfigureOptions(StoreOptions options)
            {
                options.Connection(connectionString);

                options.Schema.For<TSaga>().Identity(x => x.CorrelationId).IdStrategy(new NoOpIdGeneration());

                configure?.Invoke(options);
            }

            _configureOptions = ConfigureOptions;
        }

        public void Connection(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure = null)
        {
            void ConfigureOptions(StoreOptions options)
            {
                options.Connection(connectionFactory);

                options.Schema.For<TSaga>().Identity(x => x.CorrelationId).IdStrategy(new NoOpIdGeneration());

                configure?.Invoke(options);
            }

            _configureOptions = ConfigureOptions;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_configureOptions == null)
                yield return this.Failure("Connection", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.RegisterSingleInstance<IDocumentStore>(provider => DocumentStore.For(_configureOptions));
            configurator.RegisterSagaRepository<T, IDocumentSession, MartenSagaConsumeContextFactory<T>, MartenSagaRepositoryContextFactory<T>>();
        }
    }
}
