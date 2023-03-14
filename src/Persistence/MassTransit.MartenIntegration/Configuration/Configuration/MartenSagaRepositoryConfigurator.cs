namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Marten;
    using Marten.Schema.Identity;
    using MartenIntegration.Saga;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Npgsql;
    using Saga;


    public class MartenSagaRepositoryConfigurator<TSaga> :
        IMartenSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        Action<StoreOptions> _configureMarten;

        public void Connection(string connectionString, Action<StoreOptions> configure = null)
        {
            void ConfigureOptions(StoreOptions options)
            {
                options.Connection(connectionString);

                configure?.Invoke(options);
            }

            _configureMarten = ConfigureOptions;
        }

        public void Connection(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure = null)
        {
            void ConfigureOptions(StoreOptions options)
            {
                options.Connection(connectionFactory);

                configure?.Invoke(options);
            }

            _configureMarten = ConfigureOptions;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            if (_configureMarten != null)
            {
                configurator.AddMarten(options =>
                {
                    _configureMarten(options);
                });
            }

            configurator.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureMarten, MartenSagaRepositoryStoreOptionsConfigurator>());
            configurator.RegisterSagaRepository<T, IDocumentSession, SagaConsumeContextFactory<IDocumentSession, T>, MartenSagaRepositoryContextFactory<T>>();
        }


        class MartenSagaRepositoryStoreOptionsConfigurator :
            IConfigureMarten
        {
            public void Configure(IServiceProvider services, StoreOptions options)
            {
                options.Schema.For<TSaga>().Identity(x => x.CorrelationId).IdStrategy(new NoOpIdGeneration());
            }
        }
    }
}
