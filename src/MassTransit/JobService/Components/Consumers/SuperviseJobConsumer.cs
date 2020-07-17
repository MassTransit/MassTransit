namespace MassTransit.JobService.Components.Consumers
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Util;
    using MassTransit.Contracts.JobService;


    public class SuperviseJobConsumer<TJob> :
        IConsumer<CancelJob>,
        IConsumer<GetJobAttemptStatus>
        where TJob : class
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

            return TaskUtil.Completed;
        }
    }
}
