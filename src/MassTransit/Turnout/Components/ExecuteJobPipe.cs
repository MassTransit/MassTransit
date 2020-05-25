namespace MassTransit.Turnout.Components
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ExecuteJobPipe<TJob> :
        IPipe<JobContext<TJob>>
        where TJob : class
    {
        readonly IJobFactory<TJob> _jobFactory;

        public ExecuteJobPipe(IJobFactory<TJob> jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public async Task Send(JobContext<TJob> jobContext)
        {
            try
            {
                await jobContext.NotifyStarted().ConfigureAwait(false);

                await _jobFactory.Execute(jobContext).ConfigureAwait(false);

                await jobContext.NotifyCompleted().ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                await jobContext.NotifyCanceled("Task canceled").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await jobContext.NotifyCanceled("Operation canceled").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await jobContext.NotifyFaulted(exception).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("executeJob");
        }
    }
}
