namespace MassTransit.Persistence.Tests.ComponentTests.Postgres
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using Configuration;
    using DependencyInjection.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using PostgreSql.Configuration;


    [TestFixture]
    public class Postgres_Configurator_Tests : Postgres_Tests
    {
        [Test]
        public void Configurator_finds_other_parts()
        {
            var services = new ServiceCollection();
            var repositoryServices = new SagaRepositoryRegistrationConfigurator<InlinedSaga>(services);
            var configurator = new CustomRepositoryConfigurator<InlinedSaga>();

            configurator.UsingPostgres("my connection string", conf => conf.SetOptimisticConcurrency(s => s.Version));
            configurator.Register(repositoryServices);

            var provider = services.BuildServiceProvider();
            var instance = provider.GetRequiredService<IPostgresRepositoryConfigurator<InlinedSaga>>();

            Assert.That(instance.ConcurrencyMode, Is.EqualTo(ConcurrencyMode.Optimistic));
            Assert.That(instance.ConnectionString, Is.EqualTo("my connection string"));
            Assert.That(instance.IdentityColumnName, Is.EqualTo("CorrelationId"));
            Assert.That(instance.TableName, Is.EqualTo("tbl_saga"));
            Assert.That(instance.VersionPropertyName, Is.EqualTo("Version"));
        }

        [Test]
        public void Configurator_uses_default_conventions()
        {
            var services = new ServiceCollection();
            var repositoryServices = new SagaRepositoryRegistrationConfigurator<VersionedSaga>(services);
            var configurator = new CustomRepositoryConfigurator<VersionedSaga>();

            configurator.UsingPostgres("my connection string");
            configurator.Register(repositoryServices);

            var provider = services.BuildServiceProvider();
            var instance = provider.GetRequiredService<IPostgresRepositoryConfigurator<VersionedSaga>>();

            Assert.That(instance.ConcurrencyMode, Is.EqualTo(ConcurrencyMode.Pessimistic));
            Assert.That(instance.ConnectionString, Is.EqualTo("my connection string"));
            Assert.That(instance.IdentityColumnName, Is.EqualTo("CorrelationId"));
            Assert.That(instance.TableName, Is.EqualTo("VersionedSagas"));
            Assert.That(instance.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
        }


        [Table("tbl_saga")]
        public class InlinedSaga : ISaga
        {
            public uint Version { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}
