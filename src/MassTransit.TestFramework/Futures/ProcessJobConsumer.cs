namespace MassTransit.TestFramework.Futures;

using System;
using System.Threading.Tasks;


public class ProcessJobConsumer: IConsumer<ProcessJob>
{
    public Task Consume(ConsumeContext<ProcessJob> context)
    {
        async Task WaitAndRespond(int milliSecond)
        {
            await Task.Delay(milliSecond);
            await context.RespondAsync<ProcessJobCompleted>(new
            {
                context.Message.CorrelationId,
                ClientNumber = context.Message.JobNumber
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
