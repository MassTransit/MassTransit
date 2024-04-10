namespace MassTransit.Tests.ContainerTests.Scenarios;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TestFramework.Futures;
using TestFramework.Futures.Tests;


[TestFixture]
public class WhenAllCompletedOrFaulted : BatchFuture_Specs
{
    public WhenAllCompletedOrFaulted()
        : base(new InMemoryFutureTestFixtureConfigurator())
    {
    }
    [Test]
    public async Task Delayed_success()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new [] {"C12345", "Delay"};

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response<BatchSuccessResponse> response = await client.GetResponse<BatchSuccessResponse>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        }, timeout: RequestTimeout.After(s: 5));

        Assert.That(response.Message.ProcessedJobsNumbers, Is.EqualTo(jobNumbers));
    }

    [Test]
    public async Task Should_succeed()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new[] {"C12345", "C54321" };

        var scope = Provider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRequestClient<BatchRequest>>();

        Response<BatchSuccessResponse> response = await client.GetResponse<BatchSuccessResponse>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        }, timeout: RequestTimeout.After(s: 5));

        Assert.That(response.Message.ProcessedJobsNumbers, Is.EquivalentTo(jobNumbers));
    }

    [Test]
    public async Task Error_partially_uploaded()
    {
        var batchId = NewId.NextGuid();
        var jobNumbers = new[] {"C12345", "Error", "C54321", "Error" };

        await TestHarness.Bus.Publish<BatchRequest>(new
        {
            CorrelationId = batchId,
            JobNumbers = jobNumbers
        });

        var batchUploaded = TestHarness.Published
            .Select<BatchProcessed>()
            .Single();

        //Batch is partially successful, downstream consumers are notified of succeeded uploads
        Assert.That(batchUploaded.Context.Message.SuccessfulJobNumbers, Is.EquivalentTo(new[] { "C12345", "C54321" }));
    }
}
