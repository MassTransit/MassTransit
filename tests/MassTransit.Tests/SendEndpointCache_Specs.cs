namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Explicit]
    public class Querying_the_send_endpoint_cache_concurrently :
        InMemoryTestFixture
    {
        [Test]
        public async Task Querying_for_two_endpoints_at_the_same_time()
        {
            await Task.WhenAll(Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_a")),
                Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_b"))).ConfigureAwait(false);

            Assert.DoesNotThrowAsync(async () =>
            {
                await Task.WhenAll(Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_a")),
                    Bus.GetSendEndpoint(new Uri("loopback://localhost/queue_b")));
            });
        }
    }
}
