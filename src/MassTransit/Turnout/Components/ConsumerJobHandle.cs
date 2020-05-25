namespace MassTransit.Turnout.Components
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes.Internals.Extensions;


    public class ConsumerJobHandle<T> :
        JobHandle
        where T : class
    {
        readonly ConsumeJobContext<T> _context;
        readonly Task _task;

        public ConsumerJobHandle(ConsumeJobContext<T> context, Task task)
        {
            _context = context;
            _task = task;
        }

        public Guid JobId => _context.JobId;

        public JobStatus Status
        {
            get
            {
                return _task.Status switch
                {
                    TaskStatus.Running => JobStatus.Running,
                    TaskStatus.WaitingForChildrenToComplete => JobStatus.Running,
                    TaskStatus.Faulted => JobStatus.Faulted,
                    TaskStatus.RanToCompletion => JobStatus.RanToCompletion,
                    TaskStatus.Canceled => JobStatus.Canceled,
                    TaskStatus.Created => JobStatus.Created,
                    TaskStatus.WaitingForActivation => JobStatus.Created,
                    TaskStatus.WaitingToRun => JobStatus.Created,
                    _ => JobStatus.Created
                };
            }
        }

        public async Task Cancel()
        {
            _context.Cancel();

            try
            {
                await _task.OrTimeout(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
