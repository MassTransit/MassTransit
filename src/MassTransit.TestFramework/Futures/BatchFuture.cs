namespace MassTransit.TestFramework.Futures;

using System.Linq;


public class BatchFuture : Future<BatchRequest, BatchSuccessResponse>
{
    public BatchFuture()
    {
        ConfigureCommand(x => x.CorrelateById(context => context.Message.CorrelationId));

        SendRequests<string, ProcessJob>(x => x.JobNumbers,
                x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        CorrelationId = InVar.Id,
                        ClientNumber = context.Message
                    });
                    x.TrackPendingRequest(message => message.CorrelationId);
                })
            .OnResponseReceived<ProcessJobCompleted>(x =>
            {
                x.CompletePendingRequest(y => y.CorrelationId);
            });

        WhenAllCompleted(r => r.SetCompletedUsingInitializer(MapResponse));
        WhenAllCompletedOrFaulted(r => r.Then(PublishAllCompletedOrFaulted));
    }

    object MapResponse(BehaviorContext<FutureState> context)
    {
        var command = context.GetCommand<BatchRequest>();
        var processedJobNumbers = context
            .SelectResults<ProcessJobCompleted>()
            .Select(r => r.ClientNumber).ToList();

        return new
        {
            command.CorrelationId,
            ProcessedJobsNumbers = processedJobNumbers
        };
    }
    void PublishAllCompletedOrFaulted(BehaviorContext<FutureState> context)
    {
        var processedJobNumbers = context
            .SelectResults<ProcessJobCompleted>()
            .Select(r => r.ClientNumber).ToList();

        context.Publish<BatchProcessed>(new { SuccessfulJobNumbers = processedJobNumbers });
    }
}
