namespace MassTransit.DbTransport.Tests.PgSql;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using NUnit.Framework;
using SqlTransport.PostgreSql;


[TestFixture]
public class Migration_Specs
{
    [TestCase("host=localhost;user id=mtAdmin;password=2Legit2Quit;database=sample;")]
    [TestCase("host=messaging.postgres.database.azure.com;user id=mtAdmin@messaging;password=2Legit2Quit;database=sample")]
    [TestCase("host=messaging.server.com;user id=mtAdmin;password=2Legit2Quit;database=sample")]
    public async Task Should_parse_connection_string_into_options(string connectionString)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<SqlTransportOptions>().Configure(options =>
                {
                    options.Host = connectionStringBuilder.Host;
                    options.Port = connectionStringBuilder.Port;
                    options.Database = connectionStringBuilder.Database;
                    options.Schema = "transport";
                    options.Role = "transport";
                    options.Username = "unit_tests";
                    options.Password = "H4rd2Gu3ss!";
                    options.AdminUsername = connectionStringBuilder.Username;
                    options.AdminPassword = connectionStringBuilder.Password;
                });

                x.UsingPostgres();
            })
            .BuildServiceProvider(true);

        var opts = provider.GetRequiredService<IOptions<SqlTransportOptions>>().Value;
        var connection = PostgresSqlTransportConnection.GetDatabaseAdminConnection(opts);
        var builder = new NpgsqlConnectionStringBuilder(connection.Connection.ConnectionString);

        Assert.Multiple(() =>
        {
            Assert.That(builder.Database, Is.EqualTo("sample"));
            Assert.That(builder.Password, Is.EqualTo("2Legit2Quit"));
            Assert.That(builder.Username, Does.StartWith("mtAdmin"));
            Assert.That(connectionStringBuilder.Host, Is.EqualTo(builder.Host));
        });

        var migrationPrincipal = PostgresSqlTransportConnection.GetAdminMigrationPrincipal(opts);
        Assert.That(migrationPrincipal, Is.EqualTo("mtAdmin"));
    }
}
