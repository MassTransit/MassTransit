namespace MassTransit.DbTransport.Tests.SqlServer
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;
    using SqlTransport.SqlServer;
    using Testing;


    [TestFixture]
    public class Provisioning_the_transport_database
    {
        [Test]
        [Order(1)]
        public async Task Should_create_the_required_schema_tables_and_indices()
        {
            await using var provider = new ServiceCollection()
                .ConfigureSqlServerTransport()
                .AddMassTransitTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await using var connection = provider.GetTransportConnection();

            await connection.Open();
            await connection.Close();
        }

        [Test]
        [Order(2)]
        [Explicit]
        public async Task Should_drop_the_database_on_shutdown()
        {
            await using var provider = new ServiceCollection()
                .ConfigureSqlServerTransport(delete: true)
                .AddMassTransitTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();
        }

        readonly SqlTransportOptions _options;

        public Provisioning_the_transport_database()
        {
            _options = new SqlTransportOptions();
        }
    }


    public static class TestConfigurationExtensions
    {
        public static IServiceCollection ConfigureSqlServerTransport(this IServiceCollection services, bool create = true, bool delete = false)
        {
            services.AddOptions<SqlTransportOptions>().Configure(options =>
            {
                options.Host = "localhost";
                options.Database = "masstransit_transport_tests";
                options.Schema = "transport";
                options.Role = "transport";
                options.Username = "unit_tests";
                options.Password = "H4rd2Gu3ss!";
                options.AdminUsername = "sa";
                options.AdminPassword = "Password12!";
            });

            services.AddSqlServerMigrationHostedService(create, delete);

            return services;
        }

        public static SqlServerSqlTransportConnection GetTransportConnection(this IServiceProvider provider)
        {
            return SqlServerSqlTransportConnection.GetDatabaseConnection(provider.GetRequiredService<IOptions<SqlTransportOptions>>().Value);
        }
    }
}
