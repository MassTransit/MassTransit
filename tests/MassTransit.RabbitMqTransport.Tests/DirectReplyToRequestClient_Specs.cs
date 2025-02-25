namespace MassTransit.RabbitMqTransport.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture]
public class Using_the_direct_reply_to_request_client
{
    [Test]
    public async Task Should_return_the_response_directly()
    {
        await using var provider = new ServiceCollection()
            .ConfigureRabbitMqTestOptions(options =>
            {
                options.CleanVirtualHost = true;
                options.CreateVirtualHostIfNotExists = true;
            })
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<RabbitMqTransportOptions>()
                    .Configure(options => options.VHost = "test");

                x.SetKebabCaseEndpointNameFormatter();

                x.AddHandler<DirectRequest>(async context => await context.RespondAsync(new DirectResponse()));

                x.SetRabbitMqReplyToRequestClientFactory();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider();

        var harness = await provider.StartTestHarness();

        Response<DirectResponse> response = await harness.GetRequestClient<DirectRequest>().GetResponse<DirectResponse>(new DirectRequest());


        Assert.That(await harness.Consumed.Any<DirectRequest>(x => x.Exception == null));
    }


    public record DirectRequest;


    public record DirectResponse;
}
