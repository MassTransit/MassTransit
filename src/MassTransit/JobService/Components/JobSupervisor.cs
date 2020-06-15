namespace MassTransit.JobService.Components
{
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using MassTransit.Contracts.JobService;


    /// <summary>
    /// Consumer that handles the SuperviseJob message to check the status of the job
    /// </summary>
    public class JobSupervisor<T> :
        IConsumer<SuperviseJob<T>>,
        IConsumer<CancelJob>
        where T : class
    {
        readonly IJobService _service;

        public JobSupervisor(IJobService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            if (!_service.TryGetJob(context.Message.JobId, out var jobHandle))
                throw new JobNotFoundException($"The JobId {context.Message.JobId} was not found.");

            LogContext.Debug?.Log("Cancelling job: {JobId}", jobHandle.JobId);

            await jobHandle.Cancel().ConfigureAwait(false);

            _service.TryRemoveJob(jobHandle.JobId, out _);
        }

        public async Task Consume(ConsumeContext<SuperviseJob<T>> context)
        {
            if (_service.TryGetJob(context.Message.JobId, out var jobHandle))
            {
                switch (jobHandle.Status)
                {
                    case JobStatus.Created:
                    case JobStatus.Running:
                        //                        await _service.ScheduleSupervision(context, context.Message.Command, jobHandle).ConfigureAwait(false);
                        break;

                    case JobStatus.RanToCompletion:
                        _service.TryRemoveJob(jobHandle.JobId, out _);
                        break;

                    case JobStatus.Faulted:
                        _service.TryRemoveJob(jobHandle.JobId, out _);
                        break;

                    case JobStatus.Canceled:
                        _service.TryRemoveJob(jobHandle.JobId, out _);
                        break;
                }
            }
            else
                LogContext.Warning?.Log("Canceled job not found: {JobId}", context.Message.JobId);
        }
    }
}
