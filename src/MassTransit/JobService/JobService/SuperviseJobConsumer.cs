namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Messages;


    public class SuperviseJobConsumer :
        IConsumer<CancelJobAttempt>,
        IConsumer<GetJobAttemptStatus>
    {
        readonly IJobService _jobService;

        public SuperviseJobConsumer(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task Consume(ConsumeContext<CancelJobAttempt> context)
        {
            if (_jobService.TryGetJob(context.Message.JobId, out var handle))
                await handle.Cancel(JobCancellationReasons.CancellationRequested).ConfigureAwait(false);
        }

        public Task Consume(ConsumeContext<GetJobAttemptStatus> context)
        {
            if (_jobService.TryGetJob(context.Message.JobId, out var jobHandle))
            {
                return context.RespondAsync<JobAttemptStatus>(new JobAttemptStatusResponse
                {
                    JobId = context.Message.JobId,
                    AttemptId = context.Message.AttemptId,
                    Timestamp = DateTime.UtcNow,
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
