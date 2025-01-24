namespace MassTransit.DbTransport.Tests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Messages;
using Testing;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_the_request_client<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_properly_return_the_response()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));
                x.AddHandler<PingMessage, PongMessage>(async context => new PongMessage(context.Message.CorrelationId))
                    .Endpoint(e => e.AddConfigureEndpointCallback((context, cfg) =>
                    {
                        if(cfg is ISqlReceiveEndpointConfigurator configurator)
                        {
                            configurator.MaxDeliveryCount = 5;
                            configurator.DeadLetterExpiredMessages = true;
                        }
                    }));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.AutoStart = true;

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        IRequestClient<PingMessage> client = harness.GetRequestClient<PingMessage>();

        for (var i = 0; i < 5; i++)
        {
            var timer = Stopwatch.StartNew();

            var pingMessage = new PingMessage();
            Response<PongMessage> response = await client.GetResponse<PongMessage>(pingMessage);

            timer.Stop();

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));

            Console.WriteLine("Elapsed: {0}ms", timer.Elapsed.TotalMilliseconds);
        }
    }

    readonly T _configuration;

    public Using_the_request_client()
    {
        _configuration = new T();
    }
}
