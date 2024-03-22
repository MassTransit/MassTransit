namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Internals;


    public class ConsumerJobHandle<T> :
        JobHandle
        where T : class
    {
        readonly ConsumeJobContext<T> _context;
        readonly TimeSpan _jobCancellationTimeout;

        public ConsumerJobHandle(ConsumeJobContext<T> context, Task task, TimeSpan jobCancellationTimeout)
        {
            _context = context;
            _jobCancellationTimeout = jobCancellationTimeout;
            JobTask = task;
        }

        public Guid JobId => _context.JobId;
        public Task JobTask { get; }

        public async Task Cancel()
        {
            if (_context.CancellationToken.IsCancellationRequested)
                return;

            _context.Cancel();

            try
            {
                await JobTask.OrTimeout(_jobCancellationTimeout).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
