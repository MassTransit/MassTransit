namespace MassTransit.Tests;

using System.Threading.Tasks;
using NUnit.Framework;
using Util;


[TestFixture]
public class TaskExecutor_Specs
{
    [Test]
    public async Task Should_run_a_single_item()
    {
        await using var executor = new TaskExecutor(10);

        async ValueTask Method()
        {
            await Task.Delay(100);
        }

        await Task.Run(async () =>
            await executor.Run(Method));
    }

    [Test]
    public async Task Should_run_a_single_task()
    {
        await using var executor = new TaskExecutor(10);

        async Task Method()
        {
            await Task.Delay(100);
        }

        await Task.Run(async () =>
            await executor.Run(Method));
    }
}
