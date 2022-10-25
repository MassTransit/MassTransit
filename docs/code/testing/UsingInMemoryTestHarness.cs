namespace UsingInMemoryTestHarness;

using System.Threading.Tasks;
using MassTransit.Testing;
using NUnit.Framework;

[TestFixture]
public class Using_the_test_harness
{
    [Test]
    public async Task Should_be_easy()
    {
        var harness = new InMemoryTestHarness();

        await harness.Start();
        try
        {

        }
        finally
        {
            await harness.Stop();
        }
    }
}
