namespace MassTransit.DbTransport.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Provisioning_the_transport_database
    {
        [Test]
        [Order(1)]
        public async Task Should_create_the_required_schema_tables_and_indices()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
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
                .ConfigurePostgresTransport(delete: true)
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
}
