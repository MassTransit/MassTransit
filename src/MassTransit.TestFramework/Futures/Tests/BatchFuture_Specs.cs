namespace MassTransit.TestFramework.Futures.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[TestFixture]
public class BatchFuture_Specs :
    FutureTestFixture
{
    [Test]
    public async Task Should_succeed()
    {
        var batchId = NewId.NextGuid();
        string[] jobNumbers = ["C12345", "C54321"];

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response<BatchSuccessResponse> response = await client.GetResponse<BatchSuccessResponse>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        }, timeout: RequestTimeout.After(s: 5));

        Assert.That(response.Message.ProcessedJobsNumbers, Is.EqualTo(jobNumbers));
    }

    public BatchFuture_Specs(IFutureTestFixtureConfigurator testFixtureConfigurator)
        : base(testFixtureConfigurator)
    {
    }

    protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ProcessJobConsumer>();
        configurator.AddFuture<BatchFuture>();
    }
}
