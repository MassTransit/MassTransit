namespace MassTransit.JobService
{
    using System.Threading.Tasks;
    using Contracts.JobService;


    public class SuperviseJobConsumer :
        IConsumer<CancelJob>,
        IConsumer<GetJobAttemptStatus>
    {
        readonly IJobService _jobService;

        public SuperviseJobConsumer(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            if (_jobService.TryGetJob(context.Message.JobId, out var handle))
                await handle.Cancel();
        }

        public Task Consume(ConsumeContext<GetJobAttemptStatus> context)
        {
            if (_jobService.TryGetJob(context.Message.JobId, out var jobHandle))
            {
                return context.RespondAsync<JobAttemptStatus>(new
                {
                    context.Message.JobId,
                    context.Message.AttemptId,
                    InVar.Timestamp,
                    Status = jobHandle.JobTask.Status switch
                    {
                        TaskStatus.RanToCompletion => JobStatus.Completed,
                        TaskStatus.Faulted => JobStatus.Faulted,
                        TaskStatus.Canceled => JobStatus.Canceled,
                        _ => JobStatus.Running
                    }
                });
            }

            LogContext.Debug?.Log("CheckJobStatus, job not found: {JobId}", context.Message.JobId);

            return Task.CompletedTask;
        }
    }
}
