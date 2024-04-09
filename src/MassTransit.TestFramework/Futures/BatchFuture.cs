namespace MassTransit.TestFramework.Futures;

using System.Collections.Generic;
using System.Linq;


public class BatchFuture :
    Future<BatchRequest, BatchCompleted, BatchFaulted>
{
    public BatchFuture()
    {
        ConfigureCommand(x => x.CorrelateById(context => context.Message.CorrelationId));

        SendRequests<string, ProcessBatchItem>(x => x.JobNumbers,
                x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        CorrelationId = InVar.Id,
                        JobNumber = context.Message
                    });
                    x.TrackPendingRequest(message => message.CorrelationId);
                })
            .OnResponseReceived<ProcessBatchItemCompleted>(x =>
            {
                x.CompletePendingRequest(y => y.CorrelationId);
            });

        WhenAllCompleted(r => r.SetCompletedUsingInitializer(MapResponse));
        WhenAllCompletedOrFaulted(r => r.SetFaultedUsingInitializer(MapResponse));
    }

    object MapResponse(BehaviorContext<FutureState> context)
    {
        var command = context.GetCommand<BatchRequest>();
        List<string> processedJobNumbers = context
            .SelectResults<ProcessBatchItemCompleted>()
            .Select(r => r.JobNumber).ToList();

        return new
        {
            command.CorrelationId,
            ProcessedJobsNumbers = processedJobNumbers
        };
    }
}
