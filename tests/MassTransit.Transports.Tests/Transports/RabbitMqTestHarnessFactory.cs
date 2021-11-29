namespace MassTransit.Transports.Tests.Transports
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Testing;


    public class RabbitMqTestHarnessFactory :
        ITestHarnessFactory
    {
        public async Task<BusTestHarness> CreateTestHarness()
        {
            var harness = new RabbitMqTestHarness();

            await EnsureVirtualHostExists(harness);

            return harness;
        }

        static async Task EnsureVirtualHostExists(RabbitMqTestHarness harness)
        {
            var name = harness.GetHostSettings().VirtualHost;
            if (string.IsNullOrWhiteSpace(name) || name == "/")
                return;

            using var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{harness.Username}:{harness.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var requestUri = new UriBuilder("http", harness.HostAddress.Host, 15672, $"api/vhosts/{name}").Uri;
            await client.PutAsync(requestUri, new StringContent("{}", Encoding.UTF8, "application/json"));
        }
    }
}
