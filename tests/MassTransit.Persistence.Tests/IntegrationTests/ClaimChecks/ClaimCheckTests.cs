namespace MassTransit.Persistence.Tests.IntegrationTests.ClaimChecks
{
    using System.Security.Cryptography;
    using Connectors;
    using Microsoft.Extensions.Time.Testing;
    using NUnit.Framework;


    [TestFixture(typeof(PessimisticSqlServerConnector))]
    [TestFixture(typeof(PessimisticPostgresConnector), Explicit = true)]
    [TestFixture(typeof(PessimisticMySqlConnector), Explicit = true)]
    public class ClaimCheck_Tests<TConnector> : SagaTests<TConnector>
        where TConnector : TestConnector, new()
    {
        [Test]
        public async Task Data_already_expired_is_not_stored()
        {
            var subject = Connector.CreateMessageDataRepository(_timeProvider);

            var buffer = new byte[128 * 1024];
            Random.Shared.NextBytes(buffer);

            await using var writeStream = new MemoryStream();
            await writeStream.WriteAsync(buffer, CancellationToken.None);

            writeStream.Position = 0;
            var expected = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await subject.Put(writeStream, TimeSpan.FromSeconds(-10))
            );

            Assert.That(expected, Is.Not.Null);
            Assert.That(expected?.Message, Is.EqualTo("TTL has already expired"));
        }

        [Test]
        public async Task Data_stored_is_available_and_unmodified()
        {
            var subject = Connector.CreateMessageDataRepository(_timeProvider);
            
            var buffer = new byte[128 * 1024];
            Random.Shared.NextBytes(buffer);

            var writeHash = SHA1.HashData(buffer);

            await using var writeStream = new MemoryStream();
            await writeStream.WriteAsync(buffer, CancellationToken.None);

            writeStream.Position = 0;
            var uri = await subject.Put(writeStream);

            Assert.That(uri.ToString(), Is.Not.Null);
            Assert.That(uri.Scheme, Is.EqualTo("urn"));

            await using var readStream = await subject.Get(uri);
            var readHash = await SHA1.HashDataAsync(readStream);

            Assert.That(writeHash, Is.EqualTo(readHash));
        }

        readonly FakeTimeProvider _timeProvider;

        public ClaimCheck_Tests()
        {
            _timeProvider = new FakeTimeProvider();
            _timeProvider.SetUtcNow(DateTimeOffset.UtcNow);
        }
    }
}
