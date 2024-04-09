namespace MassTransit.Tests.ContainerTests.Scenarios;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Futures;
using TestFramework.Futures.Tests;


[TestFixture]
public class WhenAllCompletedOrFaulted :
    BatchFuture_Specs
{
    [Test]
    public async Task Delayed_success()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new[] { "C12345", "Delay" };

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response<BatchCompleted> response = await client.GetResponse<BatchCompleted>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        }, timeout: RequestTimeout.After(s: 5));

        Assert.That(response.Message.ProcessedJobsNumbers, Is.EqualTo(jobNumbers));
    }

    [Test]
    public async Task Error_partially_uploaded()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new[] { "C12345", "Error", "C54321", "Error", "C33454" };

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response response = await client.GetResponse<BatchCompleted, BatchFaulted>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        });

        switch (response)
        {
            case (_, BatchFaulted faulted):
                //Batch is partially successful, downstream consumers are notified of succeeded uploads
                Assert.That(faulted.ProcessedJobsNumbers, Is.EquivalentTo(new[] { "C12345", "C54321", "C33454" }));
                break;
            default:
                Assert.Fail("Unexpected response");
                break;
        }
    }

    [Test]
    public async Task Should_succeed()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new[] { "C12345", "C54321" };

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response<BatchCompleted> response = await client.GetResponse<BatchCompleted>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        }, timeout: RequestTimeout.After(s: 5));

        Assert.That(response.Message.ProcessedJobsNumbers, Is.EquivalentTo(jobNumbers));
    }

    public WhenAllCompletedOrFaulted()
        : base(new InMemoryFutureTestFixtureConfigurator())
    {
    }
}
