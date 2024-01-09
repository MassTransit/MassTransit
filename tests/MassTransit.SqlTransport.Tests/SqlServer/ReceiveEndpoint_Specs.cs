namespace MassTransit.DbTransport.Tests.SqlServer;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;
using UnitTests;


[TestFixture]
public class Configuring_a_receive_endpoint_without_topology
{
    [Test]
    public async Task Should_create_the_queue()
    {
        await using var provider = new ServiceCollection()
            .ConfigureSqlServerTransport()
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<MassTransitHostOptions>()
                    .Configure(options => options.StartTimeout = TimeSpan.FromSeconds(10));
                
                x.AddConsumer<TestMessageConsumer>();

                x.UsingSqlServer((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("input-queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;

                        e.ConfigureConsumer<TestMessageConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Stop();
    }
}