namespace MassTransit.Persistence.Tests.ComponentTests.SqlServer
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using Configuration;
    using DependencyInjection.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Persistence.SqlServer.Configuration;


    [TestFixture]
    public class SqlServer_Configurator_Tests : SqlServer_Tests
    {
        [Test]
        public void Configurator_finds_other_parts()
        {
            var services = new ServiceCollection();
            var repositoryServices = new SagaRepositoryRegistrationConfigurator<InlinedSaga>(services);
            var configurator = new CustomRepositoryConfigurator<InlinedSaga>();

            configurator.UsingSqlServer("my connection string", conf => conf.SetOptimisticConcurrency());
            configurator.Register(repositoryServices);

            var provider = services.BuildServiceProvider();
            var instance = provider.GetRequiredService<ISqlServerRepositoryConfigurator<InlinedSaga>>();

            Assert.That(instance.ConcurrencyMode, Is.EqualTo(ConcurrencyMode.Optimistic));
            Assert.That(instance.ConnectionString, Is.EqualTo("my connection string"));
            Assert.That(instance.IdentityColumnName, Is.EqualTo("CorrelationId"));
            Assert.That(instance.TableName, Is.EqualTo("tbl_saga"));
            Assert.That(instance.VersionColumnName, Is.EqualTo("RowVersion"));
            Assert.That(instance.VersionPropertyName, Is.EqualTo("RowVersion"));
        }

        [Test]
        public void Configurator_uses_default_conventions()
        {
            var services = new ServiceCollection();
            var repositoryServices = new SagaRepositoryRegistrationConfigurator<VersionedSaga>(services);
            var configurator = new CustomRepositoryConfigurator<VersionedSaga>();

            configurator.UsingSqlServer("my connection string");
            configurator.Register(repositoryServices);

            var provider = services.BuildServiceProvider();
            var instance = provider.GetRequiredService<ISqlServerRepositoryConfigurator<VersionedSaga>>();

            Assert.That(instance.ConcurrencyMode, Is.EqualTo(ConcurrencyMode.Pessimistic));
            Assert.That(instance.ConnectionString, Is.EqualTo("my connection string"));
            Assert.That(instance.IdentityColumnName, Is.EqualTo("CorrelationId"));
            Assert.That(instance.TableName, Is.EqualTo("VersionedSagas"));
            Assert.That(instance.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
        }


        [Table("tbl_saga")]
        public class InlinedSaga : ISaga
        {
            public TimeSpan RowVersion { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}
