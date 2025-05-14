namespace MassTransit.DapperIntegration.Tests.ComponentTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using IntegrationTests.StateMachines;
    using MassTransit.Saga;
    using MassTransit.Tests;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;
    using Saga;
    using SqlBuilders;


    [TestFixture]
    public class Configurator_Tests
    {
        [Test]
        public async Task Options_can_be_configured()
        {
            var services = new ServiceCollection();

            services.AddTransient<IConfigureOptions<DapperOptions<VersionedBehaviorSaga>>, VersionedSagaDapperConfigure>();

            services.AddMassTransit(bus =>
            {
                bus.AddSagaStateMachine<VersionedSagaStateMachine, VersionedBehaviorSaga>()
                    .DapperRepository();
            });

            var provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<IOptions<DapperOptions<VersionedBehaviorSaga>>>();
            var factory = new SagaConsumeContextFactory<DatabaseContext<VersionedBehaviorSaga>, VersionedBehaviorSaga>();

            var subject = new DapperSagaRepositoryContextFactory<VersionedBehaviorSaga>(
                options, 
                factory,
                provider
            );

            var output = await subject.CreateDatabaseContext(CancellationToken.None);

            Assert.That(VersionedSagaDapperConfigure.WasConfigured, Is.True);
            Assert.That(output, Is.TypeOf<SagaDatabaseContext<VersionedBehaviorSaga>>());

            var context = (SagaDatabaseContext<VersionedBehaviorSaga>) output;
            var actual = context.SqlFormatter.BuildLoadSql();
            var expected = @"SELECT * FROM my_table WITH (UPDLOCK, ROWLOCK) WHERE [primary_key] = @correlationId";

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Options_can_be_bound()
        {
            var services = new ServiceCollection();

            // this config would be represented inside appsettings.json as:
            //
            // "VersionedSagaOptions": {
            //   "ConnectionString": "...",
            //   "Provider": "SqlServer",
            //   "TableName": "other_table"
            // }
            services.AddTransient<IConfiguration>(sp => new ConfigurationBuilder()
                .AddInMemoryCollection([
                    new KeyValuePair<string, string>("VersionedSagaOptions:ConnectionString", LocalDbConnectionStringProvider.GetLocalDbConnectionString()),
                    new KeyValuePair<string, string>("VersionedSagaOptions:Provider", "SqlServer"),
                    new KeyValuePair<string, string>("VersionedSagaOptions:TableName", "other_table")
                ])
                .Build());

            services.AddOptions<DapperOptions<VersionedBehaviorSaga>>().BindConfiguration("VersionedSagaOptions");
            
            services.AddMassTransit(bus =>
            {
                bus.AddSagaStateMachine<VersionedSagaStateMachine, VersionedBehaviorSaga>()
                    .DapperRepository();
            });

            var provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<IOptions<DapperOptions<VersionedBehaviorSaga>>>();
            var factory = new SagaConsumeContextFactory<DatabaseContext<VersionedBehaviorSaga>, VersionedBehaviorSaga>();

            var subject = new DapperSagaRepositoryContextFactory<VersionedBehaviorSaga>(
                options,
                factory,
                provider
            );

            var output = await subject.CreateDatabaseContext(CancellationToken.None);

            Assert.That(output, Is.TypeOf<SagaDatabaseContext<VersionedBehaviorSaga>>());

            var context = (SagaDatabaseContext<VersionedBehaviorSaga>)output;
            var actual = context.SqlFormatter.BuildLoadSql();
            var expected = @"SELECT * FROM other_table WITH (UPDLOCK, ROWLOCK) WHERE [CorrelationId] = @correlationId";

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    public class VersionedSagaDapperConfigure : IConfigureOptions<DapperOptions<VersionedBehaviorSaga>>
    {
        public static bool WasConfigured;
        
        public void Configure(DapperOptions<VersionedBehaviorSaga> options)
        {
            WasConfigured = true;

            options.ConnectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
            options.Provider = DatabaseProviders.SqlServer;
            options.TableName = "my_table";
            options.IdColumnName = "primary_key";
        }
    }
}
