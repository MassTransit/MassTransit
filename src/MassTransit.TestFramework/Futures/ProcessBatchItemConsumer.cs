namespace MassTransit.TestFramework.Futures;

using System;
using System.Threading.Tasks;


public class ProcessBatchItemConsumer :
    IConsumer<ProcessBatchItem>
{
    public Task Consume(ConsumeContext<ProcessBatchItem> context)
    {
        async Task WaitAndRespond(int milliSecond)
        {
            await Task.Delay(milliSecond);
            await context.RespondAsync<ProcessBatchItemCompleted>(new
            {
                context.Message.CorrelationId,
                context.Message.JobNumber
            });
        }

        return context.Message.JobNumber switch
        {
            "Delay" => WaitAndRespond(2000),
            "Error" => throw new InvalidOperationException(),
            _ => WaitAndRespond(0)
        };
    }
}
