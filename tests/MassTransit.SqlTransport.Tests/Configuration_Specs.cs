namespace MassTransit.DbTransport.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture]
public class Configuration_Specs
{
    [Test]
    public async Task Configuring_an_ipv6_host()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<SqlTransportOptions>()
                    .Configure(options =>
                    {
                        options.Host = "::1";
                        options.Database = "test";
                        options.Schema = "transport";
                        options.Username = "user";
                        options.Password = "password";
                    });

                x.UsingSqlServer((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        Assert.That(harness.Bus.Address.Host, Is.EqualTo("[::1]"));
    }
}
